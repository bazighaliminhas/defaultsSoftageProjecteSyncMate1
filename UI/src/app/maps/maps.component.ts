import { Component, OnInit } from '@angular/core';
import { Map } from '../models/models';
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
import { PopupComponent } from '../popup/popup.component';
import { MatDialog } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';
import { MatSelectModule } from '@angular/material/select';
import { AddMapDialogComponent } from './add-map-dialog/add-map-dialog.component';
import { EditMapDialogComponent } from './edit-map-dialog/edit-map-dialog.component';

@Component({
  selector: 'maps',
  templateUrl: './maps.component.html',
  styleUrls: ['./maps.component.scss'],
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
export class MapsComponent implements OnInit {
  listOfMaps: Map[] = [];
  mapsToDisplay: Map[] = [];
  msg: string = '';
  code: number = 0;
  showSpinnerforSearch: boolean = false;
  showSpinner: boolean = false;
  options = ['Select Map', 'Id', 'Map Name', 'Map Type', 'Created Date'];
  selectedOption: string = 'Select Map';
  searchValue: string = '';
  startDate: string = '';
  endDate: string = '';
  showDataColumn: boolean = true;

  columns: string[] = [
    'id',
    'Name',
    'MapType',
    'CreatedDate',
    'Map',
    'Edit',
  ];

  constructor(private api: ApiService, private fb: FormBuilder, private toast: NgToastService, private dialog: MatDialog) {
  }

  ngOnInit(): void {
    if (this.selectedOption === 'Select Map') {
      this.getMaps();
    }
  }

  openAddMapDialog(): void {
    const dialogRef = this.dialog.open(AddMapDialogComponent, {
      width: '800px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 'saved') {
        this.getMaps();
      }
    });
  }

  openEditDialog(connectorData: any) {
    const dialogRef = this.dialog.open(EditMapDialogComponent, {
      width: '800px',
      data: connectorData
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 'updated') {
        this.getMaps();
      }
    });
  }

  get label(): string {
    return this.selectedOption === 'Select Map' ? 'Select Map' : this.selectedOption;
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


  getMaps() {
    this.showSpinnerforSearch = false;
    let stringFromDate = '';
    let stringToDate = '';

    if (this.selectedOption === 'Select Map') {
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

    this.api.getMaps(this.selectedOption, this.searchValue).subscribe({
      next: (res: any) => {
        this.listOfMaps = res.maps;
        this.msg = res.message;
        this.code = res.code;

        this.mapsToDisplay = this.listOfMaps;

        if (this.listOfMaps == null || this.listOfMaps.length === 0) {
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
