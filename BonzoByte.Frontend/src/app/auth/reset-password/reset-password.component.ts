import { CommonModule } from '@angular/common';
import { Component, OnInit, OnDestroy, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { environment } from '@env/environment';
import { AccessibleClickDirective } from '../../shared/directives/accessible-click.directive';
import { TrapFocusDirective } from '../../shared/directives/trap-focus.directive';

@Component({
    selector: 'app-reset-password',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, MatSnackBarModule, AccessibleClickDirective, TrapFocusDirective],
    templateUrl: './reset-password.component.html',
    encapsulation: ViewEncapsulation.None
})
export class ResetPasswordComponent implements OnInit, OnDestroy {
    form: FormGroup;
    loading = false;
    submitted = false;

    token: string | null = null;
    email: string | null = null;

    successMessage = '';
    errorMessage = '';

    showPassword = false;
    showConfirm = false;

    constructor(
        private fb: FormBuilder,
        private route: ActivatedRoute,
        private http: HttpClient,
        private snack: MatSnackBar,
        private router: Router
    ) {
        this.form = this.fb.group(
            {
                password: ['', Validators.required],
                confirm: ['', Validators.required],
            },
            { validators: passwordsMatchValidator }
        );
    }

    ngOnInit(): void {
        // lock body scroll
        document.body.classList.add('modal-open');

        const qp = this.route.snapshot.queryParamMap;
        this.token = qp.get('token');
        this.email = qp.get('email');

        if (!this.token || !this.email) {
            const s = new URLSearchParams(window.location.search || '');
            this.token = this.token || s.get('token');
            this.email = this.email || s.get('email');
        }
        if (!this.token || !this.email) {
            const hash = (window.location.hash || '').replace(/^#/, '');
            if (hash) {
                const hs = new URLSearchParams(hash.includes('?') ? hash.slice(hash.indexOf('?') + 1) : hash);
                this.token = this.token || hs.get('token');
                this.email = this.email || hs.get('email');
            }
        }
        if (!this.token) this.token = sessionStorage.getItem('BB_reset_token');
        if (!this.email) this.email = sessionStorage.getItem('BB_reset_email');

        if (!this.token || !this.email) {
            this.errorMessage = 'Missing token or email.';
        }
    }

    ngOnDestroy(): void {
        document.body.classList.remove('modal-open');
    }

    onSubmit(): void {
        this.submitted = true;
        if (this.form.invalid || !this.token || !this.email) return;

        this.loading = true;
        const { password } = this.form.value;

        this.http.post<{ message?: string }>(`${environment.apiUrl}/auth/reset-password`, {
            token: this.token,
            email: this.email,
            password
        }).subscribe({
            next: (res) => {
                this.successMessage = res?.message || 'Lozinka uspješno promijenjena.';
                this.errorMessage = '';
                this.snack.open(this.successMessage, '', { duration: 3000 });
            },
            error: (err) => {
                this.errorMessage = err?.error?.message || 'Greška pri promjeni lozinke.';
                this.successMessage = '';
                this.snack.open(this.errorMessage, '', { duration: 4000 });
            },
            complete: () => (this.loading = false)
        });
    }

    goToLogin(): void {
        document.body.classList.remove('modal-open');
        this.router.navigateByUrl('/').then(() => {
            window.dispatchEvent(new CustomEvent('openLogin'));
        });
    }
}

/* helper validator */
function passwordsMatchValidator(group: AbstractControl): ValidationErrors | null {
    const p = group.get('password')?.value;
    const c = group.get('confirm')?.value;
    return p && c && p === c ? null : { passwordsMismatch: true };
}