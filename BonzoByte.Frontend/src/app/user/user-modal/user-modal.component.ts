import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, EventEmitter, OnInit, OnDestroy, Output, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { User } from '../../core/models/user.model';
import { AuthService } from '../../core/services/auth.service';
import { AccessibleClickDirective } from '../../shared/directives/accessible-click.directive';
import { TrapFocusDirective } from '../../shared/directives/trap-focus.directive';
import { AbstractControl, AsyncValidatorFn, ValidationErrors, Validators } from '@angular/forms';
import { Observable, of } from 'rxjs';
import { map, catchError, switchMap } from 'rxjs/operators';
import { filter, take, finalize } from 'rxjs/operators';

/** Async validator za username zauzetost */
function usernameAvailableValidator(auth: AuthService): AsyncValidatorFn {
    return (control: AbstractControl): Observable<ValidationErrors | null> => {
        const value = (control.value || '').trim();
        if (!value) return of(null);
        // ako nije promijenjeno, ne zovi API
        if (auth.getUser()?.nickname && value === auth.getUser()!.nickname) return of(null);

        return of(value).pipe(
            debounceTime(300),
            switchMap(v => auth.checkNicknameExists(v)),
            map(exists => (exists ? { nicknameTaken: true } : null)),
            catchError(() => of(null))
        );
    };
}

@Component({
    selector: 'app-user-modal',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, MatSnackBarModule, TrapFocusDirective, AccessibleClickDirective],
    templateUrl: './user-modal.component.html',
    encapsulation: ViewEncapsulation.None
})

export class UserModalComponent implements OnInit {
    @Output() closed = new EventEmitter<void>();
    @Output() updated = new EventEmitter<User>();

    userForm!: FormGroup;
    selectedFile: File | null = null;
    previewUrl: string | null = null;

    successMessage = '';
    errorMessage = '';
    submitted = false;

    currentUser!: User;
    isLocalAccount = false;

    constructor(
        private fb: FormBuilder,
        private http: HttpClient,
        private authService: AuthService,
        private snackBar: MatSnackBar
    ) { }

    ngOnInit(): void {
        this.currentUser = this.authService.getUser()!;
        this.isLocalAccount = this.currentUser?.createdVia
            ? this.currentUser.createdVia === 'manual'
            : !this.currentUser?.googleId && !this.currentUser?.facebookId;

        this.userForm = this.fb.group({
            email: [{ value: this.currentUser.email || '', disabled: true }],
            nickname: this.fb.control(
                this.currentUser.nickname || '',
                {
                    validators: [Validators.minLength(3), Validators.maxLength(20)],
                    asyncValidators: [usernameAvailableValidator(this.authService)],
                    updateOn: 'blur'
                }
            )
        });

        if (this.currentUser.avatarUrl) this.previewUrl = this.currentUser.avatarUrl;

        // 🔒 zabrani scroll pozadine dok je modal otvoren
        document.body.classList.add('modal-open');
    }

    ngOnDestroy(): void {
        document.body.classList.remove('modal-open');
    }

    close(): void {
        document.body.classList.remove('modal-open');
        this.closed.emit();
    }

    isSaving = false;

    onSubmit(): void {
        this.submitted = true;
        this.successMessage = '';
        this.errorMessage = '';

        const nicknameCtrl = this.userForm.get('nickname');

        // Trim bez emitiranja promjene (NE pokreće validator ponovo)
        if (nicknameCtrl && typeof nicknameCtrl.value === 'string') {
            const raw = nicknameCtrl.value as string;
            const trimmed = raw.trim();
            if (trimmed !== raw) {
                nicknameCtrl.setValue(trimmed, { emitEvent: false });
            }
        }

        // Pokreni validaciju jednom (async validator će se izvršiti iako je updateOn:'blur')
        this.userForm.markAllAsTouched();
        this.userForm.updateValueAndValidity({ onlySelf: false, emitEvent: false });

        // Ako je još u PENDING stanju, pričekaj pa nastavi (jednokratno)
        if (this.userForm.pending) {
            this.userForm.statusChanges
                .pipe(filter(s => s !== 'PENDING'), take(1))
                .subscribe(() => this.onSubmit());
            return;
        }

        if (this.userForm.invalid) {
            this.errorMessage = 'Please fix validation errors before saving.';
            return;
        }

        const formData = new FormData();
        formData.append('nickname', this.userForm.get('nickname')?.value || '');
        if (this.selectedFile) formData.append('avatar', this.selectedFile);

        this.isSaving = true;
        this.authService.updateUser(formData)
            .pipe(finalize(() => (this.isSaving = false)))
            .subscribe({
                next: (user) => {
                    this.currentUser = user;
                    this.updated.emit(user);           // AppComponent zatvara modal
                    this.successMessage = 'Saved.';
                    this.errorMessage = '';
                    this.userForm.markAsPristine();
                },
                error: (err) => {
                    const msg =
                        err?.error?.message ||
                        (err?.status === 0 ? 'Cannot reach API (network/CORS).' : '') ||
                        'Update failed.';
                    this.errorMessage = msg;
                }
            });
    }

    onFileSelected(event: Event): void {
        const input = event.target as HTMLInputElement;
        if (!input.files || input.files.length === 0) return;

        const f = input.files[0];
        const okTypes = ['image/jpeg', 'image/png', 'image/webp'];
        if (f.size > 2 * 1024 * 1024) {
            this.errorMessage = 'Datoteka je prevelika (max 2MB)';
            this.selectedFile = null;
            input.value = ''; // reset input
            return;
        }
        if (!okTypes.includes(f.type)) {
            this.errorMessage = 'Dozvoljeni formati: JPG, PNG, WebP';
            this.selectedFile = null;
            input.value = '';
            return;
        }

        this.errorMessage = '';
        this.selectedFile = f;

        const reader = new FileReader();
        reader.onload = () => { this.previewUrl = reader.result as string; };
        reader.readAsDataURL(f);

        // označi formu kao “dirty” da se Save aktivira
        this.userForm.markAsDirty();
    }

    changePassword(): void {
        const email = this.userForm.get('email')?.value;
        this.authService.sendResetPasswordEmail(email).subscribe({
            next: () => {
                this.snackBar.open('Reset link poslan na vaš email.', '', {
                    duration: 5000,
                    horizontalPosition: 'right',
                    verticalPosition: 'bottom',
                    panelClass: 'success-snackbar'
                });
            },
            error: () => {
                this.snackBar.open('Greška prilikom slanja linka za reset.', '', {
                    duration: 5000,
                    horizontalPosition: 'right',
                    verticalPosition: 'bottom',
                    panelClass: 'error-snackbar'
                });
            }
        });
    }

    get nickname() {
        return this.userForm.get('nickname');
    }

    get email() {
        return this.userForm.get('email');
    }

    onAvatarError(event: Event) {
        (event.target as HTMLImageElement).src = 'assets/images/defaultUser.png';
    }
}