import { Component, OnInit } from '@angular/core';
import { Connector } from '../models/models';
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
import { PopupComponent } from '../popup/popup.component';
import { MatDialog } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';
import { MatSelectModule } from '@angular/material/select';
import { AddConnectorDialogComponent } from './add-connector-dialog/add-connector-dialog.component';
import { EditConnectorDialogComponent } from './edit-connector-dialog/edit-connector-dialog.component';
import { ConnectorsService } from '../services/connectors.service';

@Component({
  selector: 'connectors',
  templateUrl: './connectors.component.html',
  styleUrls: ['./connectors.component.scss'],
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
export class ConnectorsComponent implements OnInit {
  listOfConnectors: Connector[] = [];
  connectorsToDisplay: Connector[] = [];
  msg: string = '';
  code: number = 0;
  showSpinnerforSearch: boolean = false;
  showSpinner: boolean = false;
  options = ['Select Connector', 'Id', 'Connector Name', 'Connector Type', 'Created Date'];
  selectedOption: string = 'Select Connector';
  searchValue: string = '';
  startDate: string = '';
  endDate: string = '';
  showDataColumn: boolean = true;

  columns: string[] = [
    'id',
    'Name',
    'ConnectorType',
    'CreatedDate',
    'Data',
    'Edit',
  ];

  constructor(private ConnectorsApi: ConnectorsService, private fb: FormBuilder, private toast: NgToastService, private dialog: MatDialog) {
  }

  ngOnInit(): void {

    if (this.selectedOption === 'Select Connector') {
      this.getConnectors();
    }
  }

  openAddCustomerDialog(): void {
    const dialogRef = this.dialog.open(AddConnectorDialogComponent, {
      width: '800px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 'saved') {
        this.getConnectors();
      }
    });
  }

  openEditDialog(connectorData: any) {
    const dialogRef = this.dialog.open(EditConnectorDialogComponent, {
      width: '800px',
      data: connectorData
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 'updated') {
        this.getConnectors();
      }
    });
  }

  get label(): string {
    return this.selectedOption === 'Select Connector' ? 'Select Connector' : this.selectedOption;
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

  getConnectors() {
    this.showSpinnerforSearch = false;
    let stringFromDate = '';
    let stringToDate = '';

    if (this.selectedOption === 'Select Connector') {
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

    this.ConnectorsApi.getConnectors(this.selectedOption, this.searchValue).subscribe({
      next: (res: any) => {
        this.listOfConnectors = res.connectors;
        this.msg = res.message;
        this.code = res.code;

        this.connectorsToDisplay = this.listOfConnectors;

        if (this.listOfConnectors == null || this.listOfConnectors.length === 0) {
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
}
