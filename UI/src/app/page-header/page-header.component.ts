import { Component, EventEmitter, Output } from '@angular/core';
import { ApiService } from '../services/api.service';
import { TitleCasePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';

@Component({
    selector: 'page-header',
    templateUrl: './page-header.component.html',
    styleUrls: ['./page-header.component.scss'],
    standalone: true,
    imports: [
        MatToolbarModule,
        MatButtonModule,
        MatIconModule,
        MatMenuModule,
        RouterLink,
        TitleCasePipe,
    ],
})
export class PageHeaderComponent {
  @Output() menuClicked = new EventEmitter<boolean>();
  constructor(public api: ApiService) {}

  logOut() {
    this.api.deleteToken();
  }
}
