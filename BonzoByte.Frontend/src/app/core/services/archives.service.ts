import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

export interface DailyMatchRow {
    matchTPId: number;
    dateTime: string;
    tournamentEventName: string;
    tournamentEventCountryISO3: string;
    tournamentTypeName: string;
    tournamentLevelName: string;
    surface: string;
    player1Name: string;
    player2Name: string;
    player1Odds?: number;
    player2Odds?: number;
    winProbabilityPlayer1NN?: number;
    winProbabilityPlayer2NN?: number;
    // ... ostalo po potrebi (backend sve već vraća)
    oddsText?: string;
    probabilityText?: string;
}

export interface DailyArchiveResponse {
    date: string;   // "YYYYMMDD"
    count: number;
    matches: DailyMatchRow[];
}

@Injectable({ providedIn: 'root' })
export class ArchivesService {
    private base = `${environment.apiUrl}/archives`;

    constructor(private http: HttpClient) { }

    getDaily(yyyymmdd: string): Observable<DailyArchiveResponse> {
        return this.http.get<DailyArchiveResponse>(`${this.base}/daily/${yyyymmdd}`);
    }

    getMatchDetails(id: number | string): Observable<any> {
        return this.http.get<any>(`${this.base}/match-details/${id}`);
    }

    getLatestDaily(): Observable<{ date: string; iso: string }> {
        return this.http.get<{ date: string; iso: string }>(`${this.base}/latest-daily`);
    }

    getDateRange(): Observable<{ minDate: string; maxDate: string }> {
        return this.http.get<{ minDate: string; maxDate: string }>(`${this.base}/daterange`);
    }

    getAvailableDates(): Observable<string[]> {
        return this.http.get<string[]>(`${this.base}/available-dates`);
    }
}