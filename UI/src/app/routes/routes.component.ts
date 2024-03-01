import { Component, OnInit } from '@angular/core';
import { Routes } from '../models/models';
import { ApiService } from '../services/api.service';
import { DatePipe, NgIf, formatDate } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { FormGroup, FormControl, FormBuilder, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { NgToastService } from 'ng-angular-popup';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';
import { MatSelectModule } from '@angular/material/select';
import { AddRoutesDialogComponent } from './add-routes-dialog/add-routes-dialog.component';
import { EditRoutesDialogComponent } from './edit-routes-dialog/edit-routes-dialog.component';
import { RoutesService } from '../services/routes.service';
import { RouteLogDialogComponent } from './route-log-dialog/route-log-dialog.component';

@Component({
  selector: 'routes',
  templateUrl: './routes.component.html',
  styleUrls: ['./routes.component.scss'],
  standalone: true,
  imports: [
    MatButtonToggleModule,
    MatTableModule,
    DatePipe,
    MatCardModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    NgIf,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatTooltipModule,
    MatIconModule,
    MatProgressSpinnerModule,
    CommonModule,
    MatSelectModule,
    FormsModule,
  ],
})
export class RoutesComponent {

  listOfRoutes: Routes[] = [];
  routesToDisplay: Routes[] = [];
  msg: string = '';
  code: number = 0;
  showSpinnerforSearch: boolean = false;
  showSpinner: boolean = false;
  options = ['Select Route', 'Id', 'Source Party', 'Destination Party', 'Source Connector', 'Destination Connector', 'Party Group','Route Type', 'Created Date'];
  selectedOption: string = 'Select Route';
  searchValue: string = '';
  startDate: string = '';
  endDate: string = '';
  showDataColumn: boolean = true;

  columns: string[] = [
    'id',
    'Status',
    'SourceParty',
    'SourceConnector',
    'DestinationParty',
    'DestinationConnector',
    'PartnerGroup',
    'RouteType',
    'CreatedDate',
    'Edit'
  ];

  constructor(private api: RoutesService, private fb: FormBuilder, private toast: NgToastService, private dialog: MatDialog) {
  }

  ngOnInit(): void {
    if (this.selectedOption === 'Select Route') {
      this.getRoutes();
    }
  }

  openAddRouteDialog(): void {
    const dialogRef = this.dialog.open(AddRoutesDialogComponent, {
      width: '900px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 'saved') {
        this.getRoutes();
      }
    });
  }

  openEditDialog(connectorData: any) {
    const dialogRef = this.dialog.open(EditRoutesDialogComponent, {
      width: '900px',
      data: connectorData
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 'updated') {
        this.getRoutes();
      }
    });
  }

  get label(): string {
    return this.selectedOption === 'Select Route' ? 'Select Route' : this.selectedOption;
  }

  onSelectionChange() {
    this.searchValue = '';
  }

  getFormattedDate(date: any) {
    let year = date.getFullYear();
    let month = (1 + date.getMonth()).toString().padStart(2, '0');
    let day = date.getDate().toString().padStart(2, '0');

    return year + '-' + month + '-' + day;
  }

  getRoutes() {
    this.showSpinnerforSearch = false;
    let stringFromDate = '';
    let stringToDate = '';

    if (this.selectedOption === 'Select Route') {
      this.searchValue = 'ALL';
    }

    if (this.selectedOption === 'Created Date' && this.startDate.toLocaleString().length > 10) {
      stringFromDate = this.getFormattedDate(this.startDate);
    }
    if (this.selectedOption === 'Created Date' && this.endDate.toLocaleString().length > 10) {
      stringToDate = this.getFormattedDate(this.endDate);
    }
    if (this.selectedOption === 'Created Date' && this.startDate.toLocaleString().length > 10 && this.endDate.toLocaleString().length > 10) {
      this.searchValue = stringFromDate + '/' + stringToDate;
    }

    this.api.getRoutes(this.selectedOption, this.searchValue).subscribe({
      next: (res: any) => {
        this.listOfRoutes = res.routes;
        this.msg = res.message;
        this.code = res.code;

        this.routesToDisplay = this.listOfRoutes;

        if (this.listOfRoutes == null || this.listOfRoutes.length === 0) {
          this.toast.info({ detail: "INFO", summary: 'No data found against this search filters', duration: 5000, /*sticky: true,*/ position: 'topRight' });
          this.showSpinnerforSearch = false;

          return;
        }

        if (this.code === 200) {
          this.showSpinnerforSearch = false;
        }
        else if (this.code === 400) {
          this.toast.error({ detail: "ERROR", summary: this.msg, duration: 5000, /*sticky: true,*/ position: 'topRight' });
          this.showSpinnerforSearch = false;
        } else {
          this.toast.info({ detail: "INFO", summary: this.msg, duration: 5000, /*sticky: true,*/ position: 'topRight' });
          this.showSpinnerforSearch = false;
        }

        this.showSpinnerforSearch = false;
      },
      error: (err: any) => {
        this.toast.error({ detail: "ERROR", summary: err.message, duration: 5000, /*sticky: true,*/ position: 'topRight' });
        this.showSpinnerforSearch = false;
      },
    });


  }

  openRouteLogDialog(data: any[]): void {
    const dialogRef = this.dialog.open(RouteLogDialogComponent, {
      width: '900px',
      data: { historyData: data }
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('Dialog closed with result:', result);
    });
  }


  showRouteLog(connectorData: any) {

    this.api.getRoutelog(connectorData.id).subscribe({
      next: (res: any) => {
        if (res.code === 200) {
          this.openRouteLogDialog(res.routeLog);
        } else {
          this.showInfoToast('No Route log found');
        }
      },
      error: (err: any) => {
        this.showErrorToast('Error fetching Route log');
      }

    });
  }

  private showInfoToast(message: string): void {
    this.toast.info({
      detail: message,
      summary: 'INFO',
      duration: 5000,
      position: 'topRight'
    });
  }

  private showErrorToast(message: string): void {
    this.toast.error({
      detail: message,
      summary: 'ERROR',
      duration: 5000,
      sticky: true,
      position: 'topRight'
    });
  }




}
