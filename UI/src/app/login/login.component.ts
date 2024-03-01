import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ApiService } from '../services/api.service';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { NgIf } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatGridListModule } from '@angular/material/grid-list';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss'],
    standalone: true,
    imports: [
        MatGridListModule,
        ReactiveFormsModule,
        MatFormFieldModule,
        MatInputModule,
        NgIf,
        MatButtonModule,
        MatIconModule,
        RouterLink,
        CommonModule,
    ],
})
export class LoginComponent {
  hide = true;
  loginForm: FormGroup;
  responseMsg: string = '';
  redTextStyle = {
    color: 'red'
  }

  constructor(
    private fb: FormBuilder,
    private api: ApiService,
    private router: Router
  ) {
    this.loginForm = fb.group({
      email: fb.control('', [Validators.required, Validators.email]),
      password: fb.control('', [
        Validators.required,
        Validators.minLength(8),
        Validators.maxLength(15),
      ]),
    });
  }

  login() {
    let loginInfo = {
      email: this.loginForm.get('email')?.value,
      password: this.loginForm.get('password')?.value,
    };

    this.api.login(loginInfo).subscribe({
      next: (res: any) => {
        if (res.token === 'Invalid')
          this.responseMsg = res.message;
        else {
          this.responseMsg = '';
          this.api.saveToken(res.token);
          let isActive = this.api.getTokenUserInfo()?.status === 'ACTIVE' ? true : false;
          if (isActive) this.router.navigateByUrl('/edi/process850');
          else {
            this.responseMsg = 'You are not Active!';
            this.api.deleteToken();
          }
        }
      },
      error: (err: any) => {
        console.log('Error: ');
        console.log(err);
      },
    });
  }

  getEmailErrors() {
    if (this.Email.hasError('required')) return 'Email is required!';
    if (this.Email.hasError('email')) return 'Email is invalid.';
    return '';
  }

  getPasswordErrors() {
    if (this.Password.hasError('required')) return 'Password is required!';
    if (this.Password.hasError('minlength'))
      return 'Minimum 8 characters are required!';
    if (this.Password.hasError('maxlength'))
      return 'Maximum 15 characters are required!';
    return '';
  }

  get Email(): FormControl {
    return this.loginForm.get('email') as FormControl;
  }
  get Password(): FormControl {
    return this.loginForm.get('password') as FormControl;
  }

  cancel() {
    this.loginForm.reset();
  }
}
