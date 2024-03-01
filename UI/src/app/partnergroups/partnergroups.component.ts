import { Component, OnInit } from '@angular/core';
import { PartnerGroup } from '../models/models';
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
import { AddPartnerGroupDialogComponent } from './add-partnergroup-dialog/add-partnergroup-dialog.component';
import { EditPartnerGroupDialogComponent } from './edit-partnergroup-dialog/edit-partnergroup-dialog.component';

@Component({
  selector: 'partnergroups',
  templateUrl: './partnergroups.component.html',
  styleUrls: ['./partnergroups.component.scss'],
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
export class PartnerGroupsComponent implements OnInit {
  listOfPartnerGroups: PartnerGroup[] = [];
  partnergroupsToDisplay: PartnerGroup[] = [];
  msg: string = '';
  code: number = 0;
  showSpinnerforSearch: boolean = false;
  showSpinner: boolean = false;
  options = ['Select Partner Group', 'Id', 'Description','Source Party', 'Destination Party', 'Created Date'];
  selectedOption: string = 'Select Partner Group';
  searchValue: string = '';
  startDate: string = '';
  endDate: string = '';
  showDataColumn: boolean = true;

  columns: string[] = [
    'id',
    'Description',
    'SourceParty',
    'DestinationParty',
    'CreatedDate',
    'Edit'
  ];

  constructor(private api: ApiService, private fb: FormBuilder, private toast: NgToastService, private dialog: MatDialog) {
  }

  ngOnInit(): void {
    if (this.selectedOption === 'Select Partner Group') {
      this.getPartnerGroups();
    }
  }

  openAddPartnerGroupDialog(): void {
    const dialogRef = this.dialog.open(AddPartnerGroupDialogComponent, {
      width: '800px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 'saved') {
        this.getPartnerGroups();
      }
    });
  }

  openEditDialog(connectorData: any) {
    const dialogRef = this.dialog.open(EditPartnerGroupDialogComponent, {
      width: '800px',
      data: connectorData
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 'updated') {
        this.getPartnerGroups();
      }
    });
  }

  get label(): string {
    return this.selectedOption === 'Select Partner Group' ? 'Select Partner Group' : this.selectedOption;
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


  getPartnerGroups() {
    this.showSpinnerforSearch = false;
    let stringFromDate = '';
    let stringToDate = '';

    if (this.selectedOption === 'Select Partner Group') {
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

    this.api.getPartnerGroups(this.selectedOption, this.searchValue).subscribe({
      next: (res: any) => {
        this.listOfPartnerGroups = res.partnerGroup;
        this.msg = res.message;
        this.code = res.code;

        this.partnergroupsToDisplay = this.listOfPartnerGroups;

        if (this.listOfPartnerGroups == null || this.listOfPartnerGroups.length === 0) {
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
