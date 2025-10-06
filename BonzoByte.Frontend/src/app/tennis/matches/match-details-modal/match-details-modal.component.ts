import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-match-details-modal',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './match-details-modal.component.html',
    styleUrls: ['./match-details-modal.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MatchDetailsModalComponent {
    @Input() match!: any; // required
    @Input() loading = false;
    @Input() flagBaseUrl = '/assets/flags/';
    @Input() initialTab: TabId = 'summary';

    @Output() closed = new EventEmitter<void>();

    activeTab: TabId = 'summary';

    ngOnInit() {
        this.activeTab = this.initialTab ?? 'summary';
    }
    close() { this.closed.emit(); }

    // ======= Derived UI helpers (postojeće) =======
    get p1Name(): string { return this.match?.player1Label ?? this.match?.player1Name ?? 'Player 1'; }
    get p2Name(): string { return this.match?.player2Label ?? this.match?.player2Name ?? 'Player 2'; }
    get p1Seed(): string | null { return this.asTextOrNull(this.match?.player1Seed); }
    get p2Seed(): string | null { return this.asTextOrNull(this.match?.player2Seed); }
    get p1Flag(): string | null { return this.flagUrl(this.match?.player1CountryISO2); }
    get p2Flag(): string | null { return this.flagUrl(this.match?.player2CountryISO2); }
    get tournamentName(): string { return this.match?.tournamentEventName ?? ''; }
    get tournamentType(): string { return this.match?.tournamentTypeName ?? ''; }
    get roundName(): string { return this.match?.roundName ?? ''; }
    get surface(): string { return this.match?.matchSurfaceName || this.match?.tournamentEventSurfaceName || ''; }
    get countryISO2(): string { return this.match?.tournamentEventCountryISO2 ?? ''; }
    get countryFull(): string { return this.match?.tournamentEventCountryFull ?? ''; }
    get isFinished(): boolean { return !!this.match?.isFinished || !!this.asTextOrNull(this.match?.result); }
    get dateTimeISO(): string { return this.match?.dateTime ?? this.match?.tournamentEventDate ?? ''; }
    get result(): string | null { return this.asTextOrNull(this.match?.result); }
    get resultDetails(): string | null { return this.asTextOrNull(this.match?.resultDetails); }
    get p1SetsWon(): number | null { return this.asNumberOrNull(this.match?.p1SetsWon); }
    get p2SetsWon(): number | null { return this.asNumberOrNull(this.match?.p2SetsWon); }

    // NN Win Probabilities (primary punchline – kao vrijednosti 0..1)
    get p1Nn(): number | null { return this.asNumberOrNull(this.match?.winProbabilityPlayer1NN); }
    get p2Nn(): number | null { return this.asNumberOrNull(this.match?.winProbabilityPlayer2NN); }

    // Model chips (ostaju dostupni za kasnije)
    get p1M(): number | null { return this.asNumberOrNull(this.match?.winProbabilityPlayer1M); }
    get p2M(): number | null { return this.asNumberOrNull(this.match?.winProbabilityPlayer2M); }
    get p1SM(): number | null { return this.asNumberOrNull(this.match?.winProbabilityPlayer1SM); }
    get p2SM(): number | null { return this.asNumberOrNull(this.match?.winProbabilityPlayer2SM); }
    get p1GSM(): number | null { return this.asNumberOrNull(this.match?.winProbabilityPlayer1GSM); }
    get p2GSM(): number | null { return this.asNumberOrNull(this.match?.winProbabilityPlayer2GSM); }

    // TrueSkill Edge (default: M)
    get tsEdge(): EdgeInfo | null {
        const p1 = this.asNumberOrNull(this.match?.player1TrueSkillMeanM);
        const p2 = this.asNumberOrNull(this.match?.player2TrueSkillMeanM);
        if (p1 == null || p2 == null) return null;
        const delta = round1(p1 - p2);
        const who = delta > 0 ? 1 : delta < 0 ? 2 : 0;
        return { delta, who };
    }

    // Optionally surface-adjusted mean (GSMS{S}) if present
    getSurfaceAdjustedEdge(): EdgeInfo | null {
        const s = this.asNumberOrNull(this.match?.matchSurfaceId);
        if (!s) return null;
        const p1 = this.pick(this.match, `player1TrueSkillMeanGSMS${s}`);
        const p2 = this.pick(this.match, `player2TrueSkillMeanGSMS${s}`);
        if (p1 == null || p2 == null) return null;
        const delta = round1(p1 - p2);
        const who = delta > 0 ? 1 : delta < 0 ? 2 : 0;
        return { delta, who };
    }

    get who2Bet(): 0 | 1 | 2 {
        const v = Number(this.match?.who2Bet ?? 0);
        return v === 1 || v === 2 ? (v as 1 | 2) : 0;
    }

    get p1Streak(): number | null { return this.asNumberOrNull(this.match?.player1Streak); }
    get p2Streak(): number | null { return this.asNumberOrNull(this.match?.player2Streak); }
    get p1H2H(): number | null { return this.asNumberOrNull(this.match?.player1H2H); }
    get p2H2H(): number | null { return this.asNumberOrNull(this.match?.player2H2H); }
    get prize(): number | null { return this.asNumberOrNull(this.match?.prize); }
    get level(): string | null { return this.asTextOrNull(this.match?.tournamentLevelName); }
    get eventDateISO(): string | null { return this.asTextOrNull(this.match?.tournamentEventDate); }

    // ======= Punchline / Summary helpers (NOVO) =======

    /** Vraća NN vjerojatnosti kao 0..1; fallback parsiranje iz probabilityText */
    get nn(): { p1: number; p2: number } | null {
        let p1 = this.p1Nn;
        let p2 = this.p2Nn;

        if (p1 == null || p2 == null) {
            const text = this.asTextOrNull(this.match?.probabilityText);
            if (text) {
                const pair = this.parseProbPair(text);
                if (pair) { p1 = pair.p1; p2 = pair.p2; }
            }
        }

        if (p1 == null || p2 == null) return null;

        // Ako izgleda kao 0..100, normaliziraj na 0..1
        if (p1 > 1.0001 || p2 > 1.0001) {
            p1 = p1 / 100;
            p2 = p2 / 100;
        }
        if (!Number.isFinite(p1) || !Number.isFinite(p2)) return null;

        return { p1, p2 };
    }

    /** Tko je favorit i s kojim postotkom (0–100%) */
    get favorite(): Favorite | null {
        const probs = this.nn;
        if (!probs) return null;
        if (probs.p1 >= probs.p2) {
            return { who: 1, name: this.p1Name, probPct: Math.round(probs.p1 * 100) };
        }
        return { who: 2, name: this.p2Name, probPct: Math.round(probs.p2 * 100) };
    }

    /** Status meča: Finished / Scheduled / — */
    get statusLabel(): string {
        if (this.isFinished) return 'Finished';
        const t = Date.parse(this.dateTimeISO || '');
        if (Number.isFinite(t) && t > Date.now()) return 'Scheduled';
        return '—';
    }

    /** Chipovi za Summary */
    get chipSurface(): string { return this.surface || ''; }
    get chipLevel(): string { return this.level ?? ''; }
    get chipCountryIso2(): string { return this.countryISO2 || ''; }
    get chipCountryFlagUrl(): string | null { return this.flagUrl(this.countryISO2); }

    /** Download JSON link (proxy na .br sadržaj) */
    get downloadUrl(): string | null {
        const id = this.pickId();
        return id ? `/api/archives/match-details/${id}?download=1` : null;
    }

    // ======= Utils =======
    flagUrl(iso2?: string | null): string | null {
        if (!iso2) return null;
        return `${this.flagBaseUrl}${String(iso2).toLowerCase()}.svg`;
    }

    pick(obj: any, path: string): number | null {
        const v = obj?.[path];
        if (v == null) return null;
        const n = Number(v);
        return Number.isFinite(n) ? n : null;
    }

    asTextOrNull(v: any): string | null {
        if (v === undefined || v === null) return null;
        const s = String(v).trim();
        return s.length ? s : null;
    }

    asNumberOrNull(v: any): number | null {
        const n = Number(v);
        return Number.isFinite(n) ? n : null;
    }

    /** Detekcija ID-a: matchTPId | MatchTPId | id */
    private pickId(): number | null {
        const m: any = this.match;
        const id = m?.matchTPId ?? m?.MatchTPId ?? m?.id ?? null;
        const n = Number(id);
        return Number.isFinite(n) ? n : null;
    }

    /** Parsiraj "L – R" ili "L - R" u brojeve (može biti "0.62 – 0.38" ili "62.1 – 37.9") */
    private parseProbPair(s: string): { p1: number; p2: number } | null {
        if (!s) return null;
        const norm = s.replace(/[–—]/g, '-'); // en/em dash -> hyphen
        const parts = norm.split('-').map(p => p.trim()).filter(Boolean);
        if (parts.length !== 2) return null;
        const toNum = (t: string) => Number(String(t).replace(',', '.'));
        let a = toNum(parts[0]);
        let b = toNum(parts[1]);
        if (!Number.isFinite(a) || !Number.isFinite(b)) return null;

        // Ako su u postocima, konvertiraj u 0..1
        if (a > 1.0001 || b > 1.0001) { a = a / 100; b = b / 100; }
        return { p1: a, p2: b };
    }
}

/* ===== Types & helpers ===== */
export type TabId = 'summary' | 'stats' | 'ts' | 'wp' | 'h2h';

export interface EdgeInfo { delta: number; who: 0 | 1 | 2 }
export interface Favorite { who: 1 | 2; name: string; probPct: number }

function round1(n: number): number { return Math.round(n * 10) / 10; }