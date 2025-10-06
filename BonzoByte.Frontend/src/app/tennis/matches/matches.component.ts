import { CommonModule } from '@angular/common';
import { Component, ElementRef, HostListener, Input, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgbDatepickerModule, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import { AppComponent } from '../../app.component';
import { Match } from '../tennis.model';
import { MatchFilterModalComponent } from "./match-filter-modal/match-filter-modal.component";
import { MatchService } from './match.service';
import { MatchDetailsModalComponent } from "../matches/match-details-modal/match-details-modal.component";

@Component({
    selector: 'app-matches',
    standalone: true,
    templateUrl: './matches.component.html',
    imports: [
        CommonModule,
        FormsModule,
        NgbDatepickerModule,
        MatchFilterModalComponent,
        MatchDetailsModalComponent
    ],
    encapsulation: ViewEncapsulation.None
})

export class MatchesComponent implements OnInit, OnDestroy {
    Math = Math;
    matches: Match[] = [];
    filteredMatches: Match[] = [];
    isFiltered = false;
    filteredPage = 1;
    currentDate: Date = new Date('2022-01-01');//currentDate: Date = new Date();
    isNextDisabled = false;
    isPrevDisabled = false;
    loading = false;
    selectedDate: NgbDateStruct | null = null;
    showDatepicker = false;
    showFilterModal = false;
    selectedPlayer: number | null = null;
    selectedTournament: number | null = null;
    sortField: 'tournament' | 'date' = 'tournament';
    sortDirection: 'asc' | 'desc' = 'asc';
    filterApplied = false;
    activeDateFilter = 'all'; // 'year', 'month', 'week', 'custom'
    activeFromDate: string | null = null;
    activeToDate: string | null = null;
    activeSurfaceIds: number[] = [1, 2, 3, 4];
    activeTournamentTypeIds: number[] = [2, 4];
    activeTournamentLevelIds: number[] = [1, 2, 3, 4];
    filteredAvailableDates: string[] = [];
    showDateWarning = false;
    currentDateString: string = '';
    noMatchesForFilter = false;
    showOutOfRangeModal = false;
    @Input() minDate!: Date;
    @Input() maxDate!: Date;
    private inputDebounceTimeout: ReturnType<typeof setTimeout> | null = null;
    availableDates: string[] = [];
    selectedMatch: Match | null = null;
    private escHandler = (e: KeyboardEvent) => this.handleEscapeKey(e);

    @ViewChild('dateInput') dateInputRef!: ElementRef<HTMLInputElement>
    @HostListener('document:click', ['$event'])

    onClickOutside(event: MouseEvent): void {
        const target = event.target as HTMLElement;
        const isDatePicker = target.closest('.datepicker-wrapper');
        const isIcon = target.closest('.datepicker-toggle');

        if (!isDatePicker && !isIcon) {
            this.showDatepicker = false;
        }
    }
    constructor(private matchService: MatchService, private translate: TranslateService, private appComponent: AppComponent) { }

    // mapira specijalne/blank kodove na nešto što flag-icons zna prikazati
    flagCode(iso2?: string | null): string {
        const c = (iso2 || '').toUpperCase().trim();
        if (!c || c === 'WD' || c === 'WLD' || c === 'XX' || c === 'XW' || c === '-') return 'UN';
        // sitni aliasi, ako se pojave
        if (c === 'UK') return 'GB';   // flag-icons koristi 'gb'
        return c;
    }

    /** yyyy-MM-dd u LOKALNOJ zoni (bez UTC shiftanja) */
    private ymdLocal(d: Date): string {
        const y = d.getFullYear();
        const m = String(d.getMonth() + 1).padStart(2, '0');
        const day = String(d.getDate()).padStart(2, '0');
        return `${y}-${m}-${day}`;
    }

    /** YYYYMMDD za backend, iz lokalnog datuma */
    private ymdCompact(d: Date): string {
        return this.ymdLocal(d).replace(/-/g, '');
    }

    private daySourceMatches: Match[] = [];

    private static readonly SURFACE_MAP: Record<string, number> = {
        indoors: 1, indoor: 1, clay: 2, grass: 3, hard: 4
    };
    private static readonly TYPE_MAP: Record<string, number> = {
        ATP: 2, WTA: 4
    };
    private static readonly LEVEL_MAP: Record<string, number> = {
        '> 50,000$': 1, 'Cup': 2, 'Qualifications': 3, '< 50,000$': 4
    };

    ngOnInit(): void {
        document.addEventListener('keydown', this.escHandler);
        this.loading = true;

        // 1) Raspon datuma iz .br foldera
        this.matchService.getArchiveDateRange().subscribe({
            next: ({ minDate, maxDate }) => {
                if (!minDate || !maxDate) {
                    // nema datoteka
                    this.matches = [];
                    this.loading = false;
                    return;
                }

                this.minDate = new Date(minDate);
                this.maxDate = new Date(maxDate);

                // zadnji dan s podacima
                this.currentDate = this.maxDate;
                this.currentDateString = this.ymdLocal(this.currentDate);

                // 2) Svi dostupni dani (za strelice i ručni izbor)
                this.matchService.getArchiveAvailableDates().subscribe({
                    next: (dates) => {
                        this.availableDates = dates; // ISO "yyyy-MM-dd"
                        // učitaj mečeve za currentDate
                        this.loadMatchesForDate(this.currentDate);
                        this.loading = false;
                    },
                    error: (err) => {
                        console.error('❌ Failed to load available archive dates:', err);
                        this.loading = false;
                    }
                });
            },
            error: (err) => {
                console.error('❌ Failed to load archive date range:', err);
                this.loading = false;
            }
        });
    }

    ngOnDestroy(): void {
        document.removeEventListener('keydown', this.escHandler);
    }

    formatDate(date: Date): string {
        return this.ymdCompact(date);
    }

    scrollToTop(): void {
        const container = document.querySelector('.table-wrapper');
        if (container) {
            container.scrollTop = 0;
        }
    }

    handleEscapeKey(event: KeyboardEvent): void {
        if (event.key === 'Escape') {
            this.closeDateWarningModal?.();
            if (this.selectedMatch) this.closeMatchModal();
        }
    }

    loadMatchesForDate(date: Date): void {
        this.loading = true;

        const correctedDate = this.correctDateIfOutOfBounds(date);
        this.currentDate = correctedDate;
        this.currentDateString = this.ymdLocal(correctedDate);
        this.noMatchesForFilter = false;

        if (correctedDate.getTime() !== date.getTime()) {
            this.showDateOutOfRangeModal();
        }

        const formattedDate = this.formatDate(correctedDate); // YYYYMMDD

        this.matchService.getDailyMatches(formattedDate).subscribe({
            next: (matches) => {
                const data = matches || [];

                // spremi “izvor” za taj dan
                this.daySourceMatches = data;

                // što prikazujemo (ovisno o aktivnim filterima)
                this.matches = this.isFiltered
                    ? this.filterMatchesByActiveFilters(this.daySourceMatches)
                    : this.daySourceMatches;

                this.sortMatches();
                this.checkAdjacentDaysAvailability(correctedDate);
                this.noMatchesForFilter = this.isFiltered && this.matches.length === 0;
                this.loading = false;
            },
            error: (err) => {
                console.error('❌ Error loading matches:', err);
                this.matches = [];
                this.loading = false;
                this.isPrevDisabled = true;
                this.isNextDisabled = true;
            }
        });
    }

    sortMatches(): void {
        this.matches.sort((a, b) => {
            let valA: string | number = '';
            let valB: string | number = '';

            if (this.sortField === 'tournament') {
                valA = this.removeRomanSuffix(a.tournamentEventName || '').toLowerCase();
                valB = this.removeRomanSuffix(b.tournamentEventName || '').toLowerCase();
            } else if (this.sortField === 'date') {
                valA = new Date(a.dateTime).getTime();
                valB = new Date(b.dateTime).getTime();
            }

            const compare = valA < valB ? -1 : valA > valB ? 1 : 0;
            return this.sortDirection === 'asc' ? compare : -compare;
        });
    }

    toggleSort(field: 'tournament' | 'date'): void {
        if (this.sortField === field) {
            this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
        } else {
            this.sortField = field;
            this.sortDirection = 'asc';
        }
        this.sortMatches();
        this.applyActiveFiltersToCurrentDay();
    }

    onNativeDateChange(event: Event): void {
        const input = event.target as HTMLInputElement;
        const rawDate = new Date(input.value);

        if (isNaN(rawDate.getTime())) return;

        const formatted = rawDate.toISOString().split('T')[0];

        if (this.isFiltered && this.filteredAvailableDates.length > 0) {
            const isInsideFilter = this.filteredAvailableDates.includes(formatted);
            if (!isInsideFilter) {
                this.showOutOfFilterRangeModal();

                // Resetiraj Angular property koji se bind-a
                const currentDateFormatted = this.currentDate
                    ? this.formatDateForInput(this.currentDate)
                    : '';
                console.log('Resetting input to last valid date:', this.currentDate, currentDateFormatted);
                this.currentDateString = currentDateFormatted;

                // I ručno postavi value DOM elementa
                if (this.dateInputRef?.nativeElement) {
                    this.dateInputRef.nativeElement.value = this.currentDateString;
                }

                return;
            }
        }

        // Ako je sve ok, nastavi normalno
        this.loadMatchesForDate(rawDate);
    }

    onDateInputChanged(event: Event): void {
        const input = event.target as HTMLInputElement;

        if (this.inputDebounceTimeout) {
            clearTimeout(this.inputDebounceTimeout);
        }

        this.inputDebounceTimeout = setTimeout(() => {
            const rawDate = new Date(input.value);

            if (isNaN(rawDate.getTime())) return;

            const formatted = rawDate.toISOString().split('T')[0];

            if (this.isFiltered && this.filteredAvailableDates.length > 0) {
                const isInsideFilter = this.filteredAvailableDates.includes(formatted);
                if (!isInsideFilter) {
                    this.showOutOfFilterRangeModal();

                    const currentDateFormatted = this.ymdLocal(this.currentDate);
                    this.currentDateString = currentDateFormatted;

                    if (this.dateInputRef?.nativeElement) {
                        this.dateInputRef.nativeElement.value = currentDateFormatted;
                    }

                    return;
                }
            }

            // Ako nije filtrirano, ali korisnik upisao datum izvan min/max — korigiraj
            if (!this.isFiltered) {
                const corrected = this.correctDateIfOutOfBounds(rawDate);

                if (corrected.getTime() !== rawDate.getTime()) {
                    this.showDateOutOfRangeModal();

                    const correctedStr = this.ymdLocal(corrected);
                    this.currentDate = corrected;
                    this.currentDateString = correctedStr;

                    if (this.dateInputRef?.nativeElement) {
                        this.dateInputRef.nativeElement.value = correctedStr;
                    }

                    this.loadMatchesForDate(corrected); // već ažurira .matches

                    this.checkAdjacentDaysAvailability(corrected); // ⬅️ DODAJ OVO

                    return;
                }
            }

            this.loadMatchesForDate(rawDate);
        }, 500); // ⏳ debounce 500ms
    }

    previousDay(): void {
        const list = (this.isFiltered && this.filteredAvailableDates.length > 0)
            ? this.filteredAvailableDates
            : this.availableDates;

        const currentStr = this.ymdLocal(this.currentDate);
        const index = list.findIndex(d => d.trim() === currentStr);
        if (index > 0) {
            const prevDate = new Date(list[index - 1].trim());
            this.currentDate = prevDate;
            this.currentDateString = this.ymdLocal(prevDate);
            this.filteredPage = 1;
            this.loadMatchesForDate(prevDate);
        }
        this.scrollToTop();
    }

    nextDay(): void {
        const list = (this.isFiltered && this.filteredAvailableDates.length > 0)
            ? this.filteredAvailableDates
            : this.availableDates;

        const currentStr = this.ymdLocal(this.currentDate);
        const index = list.findIndex(d => d.trim() === currentStr);
        if (index >= 0 && index < list.length - 1) {
            const nextDate = new Date(list[index + 1].trim());
            this.currentDate = nextDate;
            this.currentDateString = this.ymdLocal(nextDate);
            this.filteredPage = 1;
            this.loadMatchesForDate(nextDate);
        }
        this.scrollToTop();
    }

    getLocalizedDateTime(date: string | Date): string {
        const parsedDate = typeof date === 'string' ? new Date(date) : date;
        if (isNaN(parsedDate.getTime())) return '';

        const pad = (n: number) => n.toString().padStart(2, '0');

        const day = pad(parsedDate.getDate());
        const month = pad(parsedDate.getMonth() + 1); // Mjeseci su 0-indeksirani
        const year = parsedDate.getFullYear();
        const hours = pad(parsedDate.getHours());
        const minutes = pad(parsedDate.getMinutes());

        return `${day}.${month}.${year} ${hours}:${minutes}`;
    }

    formatOdds(value: number | null | undefined): string {
        if (value == null || isNaN(value)) return '';
        return value.toFixed(2);
    }

    formatPercent(value: number | null | undefined): string {
        if (value == null || isNaN(value)) return '';
        return (value * 100).toFixed(0) + '%';
    }

    openMatchModal(match: Match): void {
        this.selectedMatch = match;
    }

    closeMatchModal(): void {
        this.selectedMatch = null;
    }

    openFilterModal(): void {
        this.showFilterModal = true;
    }

    closeFilterModal(): void {
        this.showFilterModal = false;
    }

    openPlayerModal(player: number) {
        this.selectedPlayer = player;
    }

    closePlayerModal() {
        this.selectedPlayer = null;
    }

    openTournamentModal(tournament: number) {
        this.selectedTournament = tournament;
    }

    closeTournamentModal() {
        this.selectedTournament = null;
    }

    formatResult(raw: string | number | null | undefined): string {
        if (raw == null) return '';
        const s = String(raw).trim();

        // već formatirano ili specijalni ishod
        if (!s) return '';
        if (s.includes(':')) return s;         // npr. "2:1"
        if (/^(RET|W[OW]|ABD|CAN|DEF)$/i.test(s)) return s; // posebni slučajevi

        // "21", "2-1", "02-01" → "2:1"
        const m = s.match(/^(\d{1,2})\D?(\d{1,2})$/);
        if (m) return `${parseInt(m[1], 10)}:${parseInt(m[2], 10)}`;

        return s; // fallback
    }

    formatResultDetails(raw?: string | null): string {
        if (!raw) return '';
        return raw
            .split(/\s+/)
            .filter(Boolean)
            .map(tok => {
                if (tok.includes(':')) return tok;                 // već formatirano
                // "62" / "76(3)" → "6:2" / "7:6(3)"
                const m1 = tok.match(/^(\d{1,2})(\d{1,2})(\([^)]+\))?$/);
                if (m1) return `${parseInt(m1[1], 10)}:${parseInt(m1[2], 10)}${m1[3] || ''}`;
                // "10-7" / "10–7" → "10:7"
                const m2 = tok.match(/^(\d{1,2})[-–](\d{1,2})$/);
                if (m2) return `${parseInt(m2[1], 10)}:${parseInt(m2[2], 10)}`;
                // specijalni tokeni (RET, WO, ...)
                return tok;
            })
            .join(' ');
    }

    removeRomanSuffix(name: string): string {
        return name.replace(/\s*(\([IVXLCDM]{1,4}\)|[IVXLCDM]{1,4})\s*$/, '').trim();
    }

    decodeHtmlEntities(value: string): string {
        const textarea = document.createElement('textarea');
        textarea.innerHTML = value;
        return textarea.value;
    }

    onFilterApplied(filter: {
        startDate: string | null;
        endDate: string | null;
        option: string;
        surfaceIds: number[];
        tournamentTypeIds: number[];
        tournamentLevelIds: number[];
    }): void {
        console.log('🧪 onFilterApplied (incoming):', filter);

        // spremi aktivne liste
        this.activeDateFilter = filter.option;              // informativno; datume ignoriramo (arhiva je per-dan)
        this.activeFromDate = filter.startDate;
        this.activeToDate = filter.endDate;

        this.activeSurfaceIds = (filter.surfaceIds || []).map(Number);
        this.activeTournamentTypeIds = (filter.tournamentTypeIds || []).map(Number);
        this.activeTournamentLevelIds = (filter.tournamentLevelIds || []).map(Number);

        const allSurfaces = [1, 2, 3, 4];
        const allTypes = [2, 4];
        const allLevels = [1, 2, 3, 4];

        const isDefaultDate = filter.option === 'all';
        const isAllSurfaces = this.activeSurfaceIds.length === allSurfaces.length && allSurfaces.every(id => this.activeSurfaceIds.includes(id));
        const isAllTypes = this.activeTournamentTypeIds.length === allTypes.length && allTypes.every(id => this.activeTournamentTypeIds.includes(id));
        const isAllLevels = this.activeTournamentLevelIds.length === allLevels.length && allLevels.every(id => this.activeTournamentLevelIds.includes(id));

        this.isFiltered = !(isDefaultDate && isAllSurfaces && isAllTypes && isAllLevels);
        this.filterApplied = this.isFiltered;

        // PRIKAŽI filtrirano/nefiltrirano iz “daySourceMatches”
        this.matches = this.isFiltered
            ? this.filterMatchesByActiveFilters(this.daySourceMatches)
            : this.daySourceMatches;

        this.sortMatches();
        this.noMatchesForFilter = this.isFiltered && this.matches.length === 0;
        this.filteredPage = 1;
        this.showFilterModal = false;
    }

    private normalizeSurfaceId(m: any): number | null {
        const v = m.surfaceId ?? m.surface_id ?? null;
        if (v !== null && v !== undefined && !Number.isNaN(+v)) return +v;
        const s = m.surface ?? m.courtSurface ?? m.surfaceName;
        if (s) {
            const key = String(s).toLowerCase().trim();
            return MatchesComponent.SURFACE_MAP[key] ?? null;
        }
        return null;
    }

    private normalizeTypeId(m: any): number | null {
        const v = m.tournamentTypeId ?? m.typeId ?? null;
        if (v !== null && v !== undefined && !Number.isNaN(+v)) return +v;
        const s = m.tournamentType ?? m.series ?? m.tour ?? m.category;
        if (s) {
            const key = String(s).toUpperCase().trim();
            return MatchesComponent.TYPE_MAP[key] ?? null;
        }
        return null;
    }

    private normalizeLevelId(m: any): number | null {
        const v = m.tournamentLevelId ?? m.levelId ?? null;
        if (v !== null && v !== undefined && !Number.isNaN(+v)) return +v;
        const s = m.tournamentLevel ?? m.level ?? m.eventLevel;
        if (s) {
            const key = String(s).trim();
            return MatchesComponent.LEVEL_MAP[key] ?? null;
        }
        return null;
    }

    private filterMatchesByActiveFilters(source: Match[]): Match[] {
        // ako nema filtera, vrati izvor
        if (!this.isFiltered) return source || [];

        const sSet = new Set((this.activeSurfaceIds || []).map(Number));
        const tSet = new Set((this.activeTournamentTypeIds || []).map(Number));
        const lSet = new Set((this.activeTournamentLevelIds || []).map(Number));

        return (source || []).filter((m: any) => {
            // SURFACE
            if (sSet.size > 0) {
                const sId = this.normalizeSurfaceId(m);
                if (sId == null || !sSet.has(sId)) return false;
            }
            // TYPE
            if (tSet.size > 0) {
                const tId = this.normalizeTypeId(m);
                if (tId == null || !tSet.has(tId)) return false;
            }
            // LEVEL
            if (lSet.size > 0) {
                const lId = this.normalizeLevelId(m);
                if (lId == null || !lSet.has(lId)) return false;
            }
            return true;
        });
    }

    private applyActiveFiltersToCurrentDay(): void {
        // filtriraj matches u memoriji
        this.filteredMatches = this.filterMatchesByActiveFilters(this.matches);
        this.noMatchesForFilter = this.isFiltered && this.filteredMatches.length === 0;

        // ako koristiš *ngFor s (isFiltered ? filteredMatches : matches), ovo je dovoljno
        // navigacija po datumima ostaje na availableDates; po promjeni dana opet primjenjujemo filter (vidi točku 3)
    }

    generateDateRange(start: string, end: string): string[] {
        const dates: string[] = [];
        const current = new Date(start);
        const endDate = new Date(end);

        while (current <= endDate) {
            const dateStr = current.toISOString().split('T')[0];
            if (this.availableDates.includes(dateStr)) {
                dates.push(dateStr);
            }
            current.setDate(current.getDate() + 1);
        }

        return dates;
    }

    clearFilter(): void {
        this.isFiltered = false;
        this.filterApplied = false;
        this.matches = this.daySourceMatches;
        this.sortMatches();
        this.filteredPage = 1;
        this.noMatchesForFilter = false;
    }

    onFilterReset(): void {
        this.filterApplied = false;
        this.activeDateFilter = 'all';
        this.activeFromDate = null;
        this.activeToDate = null;
        this.clearFilter();
    }

    isFilterDefault(): boolean {
        const allSurfacesSelected = this.activeSurfaceIds.length === 4 && [1, 2, 3, 4].every(id => this.activeSurfaceIds.includes(id));
        const allTournamentTypeIds = this.activeTournamentTypeIds.length === 2 && [2, 4].every(id => this.activeTournamentTypeIds.includes(id));
        const allTournamentLevelIds = this.activeTournamentLevelIds.length === 4 && [1, 2, 3, 4].every(id => this.activeTournamentLevelIds.includes(id));
        const dateIsAll = this.activeDateFilter === 'all';
        return allSurfacesSelected && allTournamentTypeIds && allTournamentLevelIds && dateIsAll;
    }

    correctDateIfOutOfBounds(date: Date): Date {
        if (date < this.minDate) return this.minDate;
        if (date > this.maxDate) return this.maxDate;
        return date;
    }

    showDateOutOfRangeModal(): void {
        this.showDateWarning = true;
        setTimeout(() => this.closeDateWarningModal(), 5000);
    }

    closeDateWarningModal(): void {
        this.showDateWarning = false;
    }

    checkAdjacentDaysAvailability(baseDate: Date): void {
        const currentStr = this.ymdLocal(baseDate); // 'yyyy-MM-dd'

        const list = (this.isFiltered && this.filteredAvailableDates.length > 0)
            ? this.filteredAvailableDates.map(d => d.trim())
            : this.availableDates.map(d => d.trim());

        const idx = list.indexOf(currentStr);
        this.isPrevDisabled = (idx <= 0);
        this.isNextDisabled = (idx === -1 || idx >= list.length - 1);
    }

    showOutOfFilterRangeModal(): void {
        this.showOutOfRangeModal = true;
        setTimeout(() => this.showOutOfRangeModal = false, 5000); // automatsko zatvaranje
    }

    formatDateForInput(date: Date): string {

        if (!date || isNaN(date.getTime())) {
            console.error('❌ Invalid date passed to formatDateForInput:', date);
            return 'Invalid-Date';
        }
        return this.ymdLocal(date);
    }

    cleanTournamentNameFront(name: string): string {
        if (!name) return '';
        // makni sve trailing (...) grupe
        let out = name.replace(/\s*(?:\([^()]*\)\s*)+$/g, '');
        // makni rimski broj na kraju ako je ostao
        out = out.replace(/\s*(?:[IVXLCDM]{1,4})\s*$/i, '');
        // normaliziraj razmake
        return out.replace(/\s{2,}/g, ' ').trim();
    }

    leftOf(text?: string | null): string {
        return this.splitPair(text)[0];
    }

    rightOf(text?: string | null): string {
        return this.splitPair(text)[1];
    }

    hasBoth(text?: string | null): boolean {
        const [l, r] = this.splitPair(text);
        return !!(l && r);
    }

    /** Poravnaj broj na fiksnu širinu: intWidth znamenki prije točke + fracWidth decimala.
     *  figure space (U+2007) zauzima širinu znamenke ali je “prazan”, pa je 01.23 jednako široko kao 1.23 */
    private alignFixed(value: string, intWidth: number, fracWidth: number): string {
        const v = Number(String(value).replace(',', '.'));
        if (!Number.isFinite(v)) return '';
        const [intPartRaw, fracPartRaw = ''] = v.toFixed(fracWidth).split('.');
        const intPart = intPartRaw; // nema minusa u tvojim podacima
        const padCount = Math.max(0, intWidth - intPart.length);
        const FIG = '\u2007'; // figure space = širina znamenke
        return FIG.repeat(padCount) + intPart + '.' + fracPartRaw;
    }

    private splitPair(text?: string | null): [string, string] {
        if (!text) return ['', ''];
        const parts = text.includes('–') ? text.split('–') : text.split('-');
        return [(parts[0] || '').trim(), (parts[1] || '').trim()];
    }

    private toFixedParts(v: string, frac = 2): [string, string] {
        const n = Number(String(v).replace(',', '.'));
        if (!Number.isFinite(n)) return ['', ''];
        const s = n.toFixed(frac);
        const [i, f = ''] = s.split('.');
        return [i, f];
    }

    /* nevidljive vodeće nule do targetWidth: <span class="ghost">00</span> */
    private ghostPadInt(intPart: string, targetWidth: number): string {
        const need = Math.max(0, targetWidth - intPart.length);
        return need ? `<span class="ghost">${'0'.repeat(need)}</span>${intPart}` : intPart;
    }

    /* ODS — prazno ako obje strane 0.00; širine 2+2 */
    formatOddsHTML(text?: string | null): string {
        const [l, r] = this.splitPair(text);
        const [li, lf] = this.toFixedParts(l, 2);
        const [ri, rf] = this.toFixedParts(r, 2);
        const bothZero = li === '0' && lf === '00' && ri === '0' && rf === '00';
        if (bothZero) return '';
        const L = `${this.ghostPadInt(li, 2)}.${lf}`;
        const R = `${this.ghostPadInt(ri, 2)}.${rf}`;
        return `<span class="dual dual-odds"><span class="l">${L}</span><span class="mid">–</span><span class="r">${R}</span></span>`;
    }

    /* PROB — širine 3+2 (do 100.xx) */
    formatProbHTML(text?: string | null): string {
        const [l, r] = this.splitPair(text);
        const [li, lf] = this.toFixedParts(l, 2);
        const [ri, rf] = this.toFixedParts(r, 2);
        const L = `${this.ghostPadInt(li, 3)}.${lf}`;
        const R = `${this.ghostPadInt(ri, 3)}.${rf}`;
        return `<span class="dual dual-prob"><span class="l">${L}</span><span class="mid">–</span><span class="r">${R}</span></span>`;
    }
}