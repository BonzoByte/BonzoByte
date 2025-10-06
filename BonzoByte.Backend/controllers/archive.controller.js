import fs from 'fs/promises';
import path from 'path';

// ⚠️ hardkodirano po tvojoj želji (možemo kasnije u .env)
const DAILY_DIR = 'd:\\BrotliArchives\\DayliMatches';

export const getLatestDailyDate = async (req, res) => {
    try {
        const files = await fs.readdir(DAILY_DIR);

        // tražimo YYYYMMDD.br
        const daily = files
            .filter(f => /^\d{8}\.br$/i.test(f))
            .sort()        // leksički rastuće
            .reverse();    // pa preokreni → najnoviji je prvi

        if (!daily.length) {
            return res.status(404).json({ message: 'No daily archives found.' });
        }

        const latestName = daily[0];         // npr. "20210224.br"
        const date = latestName.slice(0, 8); // "20210224"
        const iso = `${date.slice(0, 4)}-${date.slice(4, 6)}-${date.slice(6, 8)}`;

        return res.json({ date, iso, filename: latestName, dir: DAILY_DIR });
    } catch (err) {
        console.error('getLatestDailyDate error:', err);
        return res.status(500).json({ message: 'Failed to scan daily archive dir.', error: err.message });
    }
};
