import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccessibleClickDirective } from '../../../shared/directives/accessible-click.directive';
import { TrapFocusDirective } from '../../../shared/directives/trap-focus.directive';

@Component({
    selector: 'app-match-filter-modal',
    standalone: true,
    templateUrl: './match-filter-modal.component.html',
    imports: [
        CommonModule,
        FormsModule,
        AccessibleClickDirective,
        TrapFocusDirective
    ]
})
export class MatchFilterModalComponent implements OnChanges, OnInit, OnDestroy {
    @Output() closed = new EventEmitter<void>();
    @Output() filterApplied = new EventEmitter<{ startDate: string | null, endDate: string | null, option: string, surfaceIds: number[], tournamentTypeIds: number[], tournamentLevelIds: number[] }>();
    @Output() resetFilter = new EventEmitter<void>();
    @Output() surfaceFilterChanged = new EventEmitter<number | null>();
    @Output() tournamentTypeFilterChanged = new EventEmitter<number | null>();
    @Output() tournamentLevelFilterChanged = new EventEmitter<number | null>();
    @Input() activeDateFilter = 'all';
    @Input() activeFromDate: string | null = null;
    @Input() activeToDate: string | null = null;
    @Input() activeSurfaceIds: number[] = [];
    @Input() activeTournamentTypeIds: number[] = [];
    @Input() activeTournamentLevelIds: number[] = [];
    @Input() minDate!: Date;
    @Input() maxDate!: Date;

    dateOptions = [
        { label: 'All matches', value: 'all' },
        { label: 'Last year', value: 'year' },
        { label: 'Last month', value: 'month' },
        { label: 'Last week', value: 'week' },
        { label: 'Custom date range', value: 'custom' }
    ];

    surfaces = [
        { _id: 1, surface: 'Indoors' },
        { _id: 2, surface: 'Clay' },
        { _id: 3, surface: 'Grass' },
        { _id: 4, surface: 'Hard' }
    ];

    tournamentTypes = [
        { _id: 2, type: 'ATP' },
        { _id: 4, type: 'WTA' },
    ];

    tournamentLevels = [
        { _id: 1, level: '> 50,000$' },
        { _id: 2, level: 'Cup' },
        { _id: 3, level: 'Qualifications' },
        { _id: 4, level: '< 50,000$' },
    ];

    /** 👇 mapiranje ID ↔ ključ (za slučaj da roditelj filtrira po stringovima) */
    private readonly SURFACE_BY_ID: Record<number, string> = { 1: 'indoors', 2: 'clay', 3: 'grass', 4: 'hard' };
    private readonly TYPE_BY_ID: Record<number, string> = { 2: 'ATP', 4: 'WTA' };
    private readonly LEVEL_BY_ID: Record<number, string> = { 1: '> 50,000$', 2: 'Cup', 3: 'Qualifications', 4: '< 50,000$' };

    selectedDateOption = 'all';
    selectedSurfaceIds: number[] = [];
    selectedTournamentTypeIds: number[] = [];
    selectedTournamentLevelIds: number[] = [];
    customFromDate: string | null = null;
    customToDate: string | null = null;

    ngOnChanges(changes: SimpleChanges): void {
        if (changes['activeDateFilter']) {
            this.selectedDateOption = this.activeDateFilter;
        }

        if (this.activeDateFilter === 'custom') {
            this.customFromDate = this.activeFromDate;
            this.customToDate = this.activeToDate;
        } else {
            this.customFromDate = null;
            this.customToDate = null;
        }

        if (changes['activeSurfaceIds']) {
            this.selectedSurfaceIds = [...this.activeSurfaceIds];
        }

        // Osiguraj default
        if (!this.selectedSurfaceIds || this.selectedSurfaceIds.length === 0) {
            this.selectedSurfaceIds = [1, 2, 3, 4];
        }

        if (changes['activeTournamentTypeIds']) {
            this.selectedTournamentTypeIds = [...this.activeTournamentTypeIds];
        }

        if (!this.selectedTournamentTypeIds || this.selectedTournamentTypeIds.length === 0) {
            this.selectedTournamentTypeIds = [2, 4];
        }

        if (changes['activeTournamentLevelIds']) {
            this.selectedTournamentLevelIds = [...this.activeTournamentLevelIds];
        }

        if (!this.selectedTournamentLevelIds || this.selectedTournamentLevelIds.length === 0) {
            this.selectedTournamentLevelIds = [1, 2, 3, 4];
        }
    }


    ngOnInit(): void {
        document.body.classList.add('modal-open');
    }

    ngOnDestroy(): void {
        document.body.classList.remove('modal-open');
    }

    onDateOptionChange(): void {
        if (this.selectedDateOption !== 'custom') {
            this.customFromDate = null;
            this.customToDate = null;
        }
    }

    onForceOptionChange(value: string) {
        // Omogući klik ako korisnik klikne već aktivnu opciju
        if (this.selectedDateOption === value) {
            this.onDateOptionChange();
        }
    }

    private calculateStartDate(option: string): string | null {
        const d = new Date();
        switch (option) {
            case 'year': d.setFullYear(d.getFullYear() - 1); break;
            case 'month': d.setMonth(d.getMonth() - 1); break;
            case 'week': d.setDate(d.getDate() - 7); break;
            default: return null; // 'all' ili 'custom'
        }
        return d.toISOString().split('T')[0];
    }

    close(): void {
        this.closed.emit();
    }

    private fmt(d: Date | null): string | null {
        if (!d) return null;
        const yyyy = d.getFullYear();
        const mm = String(d.getMonth() + 1).padStart(2, '0');
        const dd = String(d.getDate()).padStart(2, '0');
        return `${yyyy}-${mm}-${dd}`;
    }

    private calcStartFromMax(option: string): string | null {
        if (!this.maxDate) return null;
        const end = new Date(this.maxDate);
        const start = new Date(this.maxDate);
        switch (option) {
            case 'year': start.setFullYear(start.getFullYear() - 1); break;
            case 'month': start.setMonth(start.getMonth() - 1); break;
            case 'week': start.setDate(start.getDate() - 7); break;
            default: return null; // 'all' ili 'custom'
        }
        return this.fmt(start);
    }

    apply(): void {
        // 1) Datumi (emitamo NULL kad nema restrikcije)
        let startDate: string | null = null;
        let endDate: string | null = null;

        if (this.selectedDateOption === 'custom') {
            if (this.customFromDate && this.customToDate && this.customFromDate > this.customToDate) {
                [this.customFromDate, this.customToDate] = [this.customToDate, this.customFromDate];
            }
            startDate = this.customFromDate || null;
            endDate = this.customToDate || null;
        } else if (this.selectedDateOption === 'all') {
            startDate = null;
            endDate = null;
        } else {
            // relativni raspon prema DANAŠNJEM danu (kao stara verzija)
            startDate = this.calculateStartDate(this.selectedDateOption);
            endDate = new Date().toISOString().split('T')[0];
        }

        // 2) ID-ovi (osiguraj brojeve)
        const surfaceIds = this.selectedSurfaceIds.map(Number);
        const tournamentTypeIds = this.selectedTournamentTypeIds.map(Number);
        const tournamentLevelIds = this.selectedTournamentLevelIds.map(Number);

        const payload = {
            option: this.selectedDateOption,
            startDate, endDate,
            surfaceIds, tournamentTypeIds, tournamentLevelIds
        };

        // 3) DEBUG: jasni logovi
        console.groupCollapsed('%c[Filter] Emitting', 'color:#0b5; font-weight:600;');
        console.log('dateOption:', this.selectedDateOption, { startDate, endDate });
        console.table([
            { key: 'surfaceIds', values: surfaceIds.join(', ') || '(empty)' },
            { key: 'tournamentTypeIds', values: tournamentTypeIds.join(', ') || '(empty)' },
            { key: 'tournamentLevelIds', values: tournamentLevelIds.join(', ') || '(empty)' },
        ]);
        console.groupEnd();

        this.filterApplied.emit(payload);
    }

    toggleSurface(id: number): void {
        this.selectedSurfaceIds = this.selectedSurfaceIds.includes(id)
            ? this.selectedSurfaceIds.filter(x => x !== id)
            : [...this.selectedSurfaceIds, id];
        console.log('[Filter] surfaceIds →', this.selectedSurfaceIds);
    }
    toggleTournamentType(id: number): void {
        this.selectedTournamentTypeIds = this.selectedTournamentTypeIds.includes(id)
            ? this.selectedTournamentTypeIds.filter(x => x !== id)
            : [...this.selectedTournamentTypeIds, id];
        console.log('[Filter] tournamentTypeIds →', this.selectedTournamentTypeIds);
    }
    toggleTournamentLevel(id: number): void {
        this.selectedTournamentLevelIds = this.selectedTournamentLevelIds.includes(id)
            ? this.selectedTournamentLevelIds.filter(x => x !== id)
            : [...this.selectedTournamentLevelIds, id];
        console.log('[Filter] tournamentLevelIds →', this.selectedTournamentLevelIds);
    }

    reset(): void {
        this.selectedDateOption = 'all';
        this.customFromDate = null;
        this.customToDate = null;
        this.selectedSurfaceIds = [1, 2, 3, 4];
        this.selectedTournamentTypeIds = [2, 4];
        this.selectedTournamentLevelIds = [1, 2, 3, 4];
        this.resetFilter.emit();
    }

    selectAllSurfaces() { this.selectedSurfaceIds = this.surfaces.map(s => s._id); }
    clearSurfaces() { this.selectedSurfaceIds = []; }

    selectAllTypes() { this.selectedTournamentTypeIds = this.tournamentTypes.map(t => t._id); }
    clearTypes() { this.selectedTournamentTypeIds = []; }

    selectAllLevels() { this.selectedTournamentLevelIds = this.tournamentLevels.map(t => t._id); }
    clearLevels() { this.selectedTournamentLevelIds = []; }
}