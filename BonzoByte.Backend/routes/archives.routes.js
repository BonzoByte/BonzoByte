// routes/archives.routes.js
import { Router } from 'express';
import fs from 'fs';
import path from 'path';
import { brotliDecompressSync } from 'zlib';
import { S3Client, ListObjectsV2Command } from '@aws-sdk/client-s3';

const router = Router();

/* ----------------------------- Helpers ----------------------------- */

const FIGURE_SPACE = '\u2007';
const fmtNum = (val, digitsBefore = 3, decimals = 2) => {
    if (val == null || !isFinite(val)) return '';
    const s = Number(val).toFixed(decimals);
    const [int, frac] = s.split('.');
    const pad = FIGURE_SPACE.repeat(Math.max(0, digitsBefore - int.length));
    return `${pad}${int}.${frac}`;
};

const displaySurface = (name = '') => {
    const s = String(name || '').trim();
    if (!s) return '';
    return /^(?:unknown|unk|n\/a|na|none|-|0)$/i.test(s) ? '' : s;
};

const cleanTournamentName = (name = '') =>
    String(name || '')
        .replace(/\s*(?:\([^()]*\)\s*)+$/g, '')
        .replace(/\s*(?:[IVXLCDM]{1,4})\s*$/i, '')
        .replace(/\s{2,}/g, ' ')
        .trim();

const cleanIso3ForDisplay = (iso3 = '') => {
    const s = String(iso3 || '').toUpperCase().trim();
    if (!s || s === 'WLD' || s === 'NA' || s === 'N/A' || s === '---') return '';
    return /^[A-Z]{3}$/.test(s) ? s : '';
};

const playerLabel = (name = '', seed = '', iso3 = '') => {
    const seedPart = seed ? ` [${seed}]` : '';
    const iso = cleanIso3ForDisplay(iso3);
    const isoPart = iso ? ` (${iso})` : '';
    return `${name}${seedPart}${isoPart}`.trim();
};

const formatResult = (r = '') => (r && r.length === 2 ? `${r[0]}:${r[1]}` : '');

const formatResultDetails = (s = '') =>
    String(s || '')
        .split(' ')
        .filter(Boolean)
        .map(tok => {
            const m = tok.match(/^(\d)(\d)(\(.+?\))?$/);
            return m ? `${m[1]}:${m[2]}${m[3] || ''}` : tok;
        })
        .join(' ');

const dashPairAligned = (a, b, decimals = 2) => {
    if (a == null || b == null || !isFinite(a) || !isFinite(b)) {
        return { left: '', right: '', text: '' };
    }
    const left = fmtNum(a, 3, decimals);
    const right = fmtNum(b, 3, decimals);
    return { left, right, text: `${left} - ${right}` };
};

/* ------------------------------ Config ----------------------------- */

// Izvor podataka
const ARCHIVES_SOURCE = (process.env.ARCHIVES_SOURCE || 'remote').toLowerCase(); // 'remote' | 'local'

// R2 public base URL (čitanje preko HTTP-a)
const ARCHIVES_BASE_URL = process.env.ARCHIVES_BASE_URL || '';

// Lokalni fallback (ako koristiš ARCHIVES_SOURCE=local)
const DAYLI_DIR = process.env.BROTLI_DAYLI_DIR || 'd:\\BrotliArchives\\DayliMatches';
const MATCH_DETAILS_DIR = process.env.BROTLI_MATCH_DETAILS_DIR || 'd:\\BrotliArchives\\MatchDetails';

// R2 S3 (za listanje datuma)
const R2 = {
    bucket: process.env.R2_BUCKET || '',
    accountId: process.env.R2_ACCOUNT_ID || '',
    endpoint: process.env.R2_ENDPOINT || (process.env.R2_ACCOUNT_ID ? `https://${process.env.R2_ACCOUNT_ID}.r2.cloudflarestorage.com` : ''),
    accessKeyId: process.env.R2_ACCESS_KEY_ID || '',
    secretAccessKey: process.env.R2_SECRET_ACCESS_KEY || '',
};

// S3 client (samo ako su varijable postavljene)
const s3 = (R2.bucket && R2.endpoint && R2.accessKeyId && R2.secretAccessKey)
    ? new S3Client({
        region: 'auto',
        endpoint: R2.endpoint,
        forcePathStyle: true,
        credentials: { accessKeyId: R2.accessKeyId, secretAccessKey: R2.secretAccessKey },
    })
    : null;

// Minimalna pomoćna
const yyyymmddToIso = (yyyymmdd) =>
    `${yyyymmdd.slice(0, 4)}-${yyyymmdd.slice(4, 6)}-${yyyymmdd.slice(6, 8)}`;

/* --------------------------- IO utilities --------------------------- */

// Remote (HTTP) čitanje .br u Buffer
async function fetchRemoteBrToBuffer(key) {
    if (!ARCHIVES_BASE_URL) throw new Error('ARCHIVES_BASE_URL is not set');
    const url = `${ARCHIVES_BASE_URL.replace(/\/+$/, '')}/${key.replace(/^\/+/, '')}`;
    const r = await fetch(url, { method: 'GET' });
    if (!r.ok) throw new Error(`HTTP ${r.status} for ${url}`);
    const ab = await r.arrayBuffer();
    return Buffer.from(ab);
}

// Abstrakcija za čitanje .br (daily ili match)
async function readArchiveBuffer(kind, name) {
    if (ARCHIVES_SOURCE === 'local') {
        const filePath = path.join(kind === 'daily' ? DAYLI_DIR : MATCH_DETAILS_DIR, `${name}.br`);
        await fs.promises.access(filePath, fs.constants.R_OK);
        return await fs.promises.readFile(filePath);
    } else {
        const key = (kind === 'daily')
            ? `daily-matches/${name}.br`
            : `match-details/${name}.br`;
        return await fetchRemoteBrToBuffer(key);
    }
}

// Listanje dostupnih daily .br datoteka
async function listDailyBrFiles() {
    if (ARCHIVES_SOURCE === 'local') {
        const files = await fs.promises.readdir(DAYLI_DIR);
        return files.filter(f => /^\d{8}\.br$/i.test(f)).sort((a, b) => a.localeCompare(b));
    }

    // remote → preko S3 listanja (R2)
    if (!s3) throw new Error('R2 S3 is not configured for listing');
    const Prefix = 'daily-matches/';
    const keys = [];
    let ContinuationToken;

    do {
        const out = await s3.send(new ListObjectsV2Command({
            Bucket: R2.bucket,
            Prefix,
            ContinuationToken,
        }));
        (out.Contents || []).forEach(obj => {
            if (obj.Key && /\.br$/i.test(obj.Key)) keys.push(obj.Key);
        });
        ContinuationToken = out.IsTruncated ? out.NextContinuationToken : undefined;
    } while (ContinuationToken);

    // vratimo samo imena datoteka (YYYYMMDD.br)
    return keys
        .map(k => k.substring(Prefix.length))
        .filter(name => /^\d{8}\.br$/i.test(name))
        .sort((a, b) => a.localeCompare(b));
}

/* ------------------------------- Routes ---------------------------- */

// GET /api/archives/latest-daily
router.get('/latest-daily', async (_req, res) => {
    try {
        const br = await listDailyBrFiles();
        if (!br.length) return res.status(404).json({ message: 'No .br files found.' });

        br.sort((a, b) => b.localeCompare(a));
        const latest = br[0]; // npr. "20240115.br"
        const date = latest.replace(/\.br$/i, '');
        const iso = yyyymmddToIso(date);

        res.json({
            date,
            iso,
            filename: latest,
            source: ARCHIVES_SOURCE,
            baseUrl: ARCHIVES_SOURCE === 'remote' ? ARCHIVES_BASE_URL : undefined,
        });
    } catch (e) {
        console.error('❌ /archives/latest-daily error:', e);
        res.status(500).json({ message: 'Failed to list archives.' });
    }
});

// GET /api/archives/daterange  -> { minDate, maxDate }
router.get('/daterange', async (_req, res) => {
    try {
        const br = await listDailyBrFiles();
        if (!br.length) return res.status(404).json({ message: 'No .br files found.' });

        const first = br[0].replace(/\.br$/i, '');
        const last = br[br.length - 1].replace(/\.br$/i, '');
        res.json({ minDate: yyyymmddToIso(first), maxDate: yyyymmddToIso(last) });
    } catch (e) {
        console.error('❌ /archives/daterange error:', e);
        res.status(500).json({ message: 'Failed to list archives.' });
    }
});

// GET /api/archives/available-dates  -> ["YYYY-MM-DD", ...] asc
router.get('/available-dates', async (_req, res) => {
    try {
        const br = await listDailyBrFiles();
        const dates = br.map(f => yyyymmddToIso(f.replace(/\.br$/i, '')));
        res.json(dates);
    } catch (e) {
        console.error('❌ /archives/available-dates error:', e);
        res.status(500).json({ message: 'Failed to list archives.' });
    }
});

// GET /api/archives/daily/:date  (date = "YYYYMMDD")
router.get('/daily/:date', async (req, res) => {
    const { date } = req.params;
    if (!/^\d{8}$/.test(date)) return res.status(400).json({ message: 'Invalid date format.' });

    try {
        const brBuf = await readArchiveBuffer('daily', date);
        const jsonBuf = brotliDecompressSync(brBuf);
        const text = jsonBuf.toString('utf8');

        const rows = JSON.parse(text);
        if (!Array.isArray(rows)) {
            return res.status(500).json({ message: 'Archive content is not an array.' });
        }

        const matches = rows.map(m => {
            const tourNameClean = cleanTournamentName(m.tournamentEventName || '');
            const iso3 = (m.tournamentEventCountryISO3 || '').toUpperCase();
            const countrySuffix = iso3 && iso3 !== 'WLD' ? ` (${iso3})` : '';
            const tournamentTitle = tourNameClean + countrySuffix;

            const player1LabelText = playerLabel(m.player1Name || '', m.player1Seed || '', m.player1CountryISO3 || '');
            const player2LabelText = playerLabel(m.player2Name || '', m.player2Seed || '', m.player2CountryISO3 || '');

            const resultText = formatResult(m.result || '');
            const resultDetailsText = formatResultDetails(m.resultDetails || '');

            const odds = dashPairAligned(m.player1Odds, m.player2Odds, 2);
            const probs = dashPairAligned(m.winProbabilityPlayer1NN, m.winProbabilityPlayer2NN, 2);

            return {
                ...m,
                tournamentType: m.tournamentTypeName,
                tournamentLevel: m.tournamentLevelName,
                surface: displaySurface(m.matchSurfaceName || m.tournamentEventSurfaceName || ''),
                tournamentTitle,
                player1Label: player1LabelText,
                player2Label: player2LabelText,
                resultText,
                resultDetailsText,
                oddsText: odds.text,
                oddsLeft: odds.left,
                oddsRight: odds.right,
                probabilityText: probs.text,
                probLeft: probs.left,
                probRight: probs.right,
            };
        });

        res.setHeader('Cache-Control', 'public, max-age=300');
        res.json({ date, count: matches.length, matches });
    } catch (e) {
        console.error('❌ /archives/daily error:', e);
        res.status(500).json({ message: 'Failed to read/decompress archive.' });
    }
});

// GET /api/archives/match-details/:id
router.get('/match-details/:id', async (req, res) => {
    try {
        const id = String(req.params.id).trim();
        if (!/^\d{5,12}$/.test(id)) {
            return res.status(400).json({ message: 'Invalid id format.' });
        }

        const brBuf = await readArchiveBuffer('match', id);
        const rawBuf = brotliDecompressSync(brBuf);
        const text = rawBuf.toString('utf8').replace(/^\uFEFF/, '');

        if (String(req.query.download || '').toLowerCase() === '1') {
            res.setHeader('Content-Type', 'application/json; charset=utf-8');
            res.setHeader('Content-Disposition', `attachment; filename="${id}.json"`);
            res.setHeader('Cache-Control', 'public, max-age=300');
            return res.send(text);
        }

        const json = JSON.parse(text);
        res.setHeader('Cache-Control', 'public, max-age=300');
        return res.json(json);
    } catch (e) {
        console.error('❌ /archives/match-details error:', e);
        return res.status(500).json({ message: 'Failed to read/decompress archive.' });
    }
});

export default router;