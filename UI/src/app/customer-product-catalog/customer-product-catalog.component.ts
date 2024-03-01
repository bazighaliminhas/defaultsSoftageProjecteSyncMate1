import { Component, OnInit } from '@angular/core';
import { CustomerProductCatalog, HistoryCustomerProductCatalog } from '../models/models';
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
import { EditCustomerProductCatalogDialogComponent } from './edit-customer-product-catalog-dialog/edit-customer-product-catalog-dialog.component';
import { HistoryCustomerProductCatalogDialogComponent } from './history-customer-product-catalog-dialog/history-customer-product-catalog-dialog.component';

import { CustomerProductCatalogService } from '../services/customerProductCatalogDialog.service';

@Component({
  selector: 'customer-product-catalog',
  templateUrl: './customer-product-catalog.component.html',
  styleUrls: ['./customer-product-catalog.component.scss'],
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
export class CustomerProductCatalogComponent {
  listOfCustomerProductCatalog: CustomerProductCatalog[] = [];
  customerProductCatalogToDisplay: CustomerProductCatalog[] = [];
  msg: string = '';
  code: number = 0;
  showSpinnerforSearch: boolean = false;
  showSpinner: boolean = false;
  options = ['Select Customer Product Catalog', 'Id', 'ERP CustomerID', 'TCIN', 'Partner SKU', 'Item Type', 'ItemTypeID', 'Created Date'];
  selectedOption: string = 'Select Customer Product Catalog';
  searchValue: string = '';
  startDate: string = '';
  endDate: string = '';
  showDataColumn: boolean = true;
  selectedFile: File | null = null;
  isButtonDisabled: boolean = false;
  erpCustomerID: string = '';
  listOfHistoryCustomerProductCatalog: HistoryCustomerProductCatalog[] = [];
  historyCustomerProductCatalogToDisplay: HistoryCustomerProductCatalog[] = [];

  columns: string[] = [
    'id',
    'ERPCustomerID',
    'TCIN',
    'PartnerSKU',
    'ProductTitle',
    'ItemType',
    'ItemTypeID',
    'Relationship',
    'PublishStatus',
    'DataUpdatesStatus',
    'CreatedDate',
    'Edit',
  ];

  constructor(private api: CustomerProductCatalogService, private fb: FormBuilder, private toast: NgToastService, private dialog: MatDialog) {
  }

  ngOnInit(): void {
    if (this.selectedOption === 'Select Customer Product Catalog') {
      this.getCustomerProductCatalog();
    }
  }

  openEditDialog(connectorData: any) {
    const dialogRef = this.dialog.open(EditCustomerProductCatalogDialogComponent, {
      width: '900px',
      data: connectorData
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 'updated') {
        this.getCustomerProductCatalog();
      }
    });
  }

  openHistoryDialog(data: any[]): void {
    const dialogRef = this.dialog.open(HistoryCustomerProductCatalogDialogComponent, {
      width: '900px',
      data: { historyData: data }
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('Dialog closed with result:', result);
    });
  }

  onFileSelected(event: Event) {
    const inputElement = event.target as HTMLInputElement;
    if (inputElement.files) {
      this.selectedFile = inputElement.files[0];
    }
  }

  clearFile() {
    const input = document.querySelector('.file-input') as HTMLInputElement;
    if (input) {
      input.value = '';
      this.selectedFile = null;
    }
    this.showSpinner = false;
    this.isButtonDisabled = false;
  }

  get label(): string {
    return this.selectedOption === 'Select Customer Product Catalog' ? 'Select Customer Product Catalog' : this.selectedOption;
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

  getCustomerProductCatalog() {
    this.showSpinnerforSearch = false;
    let stringFromDate = '';
    let stringToDate = '';

    if (this.selectedOption === 'Select Customer Product Catalog') {
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

    this.api.getCustomerProductCatalog(this.selectedOption, this.searchValue).subscribe({
      next: (res: any) => {
        this.listOfCustomerProductCatalog = res.customerProductCatalog;
        this.msg = res.message;
        this.code = res.code;

        this.customerProductCatalogToDisplay = this.listOfCustomerProductCatalog;

        if (this.listOfCustomerProductCatalog == null || this.listOfCustomerProductCatalog.length === 0) {
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

  uploadCustomerProductCatalogFile() {
    if (this.selectedFile == null) {
      this.toast.info({ detail: "INFO", summary: 'Please Choose file for process', duration: 5000, /*sticky: true,*/ position: 'topRight' });
      return;
    }

    if (this.erpCustomerID == null || this.erpCustomerID == "") {
      this.toast.info({ detail: "INFO", summary: 'Please provide ERPCustomerID', duration: 5000, /*sticky: true,*/ position: 'topRight' });
      return;
    }

    if (this.selectedFile) {
      this.isButtonDisabled = true;
      this.showSpinner = true;

      this.api.uploadCustomerProductCatalogFile(this.selectedFile, this.erpCustomerID).subscribe(
        {
          next: (res: any) => {
            this.msg = res.message;
            this.code = res.code;
            if (this.code === 200) {
              this.toast.success({ detail: "SUCCESS", summary: 'File: [ ' + this.selectedFile?.name + ' ] processed successfully!', duration: 5000, sticky: true, position: 'topRight' });
              this.getCustomerProductCatalog();
            }
            else if (this.code === 400) {
              this.toast.warning({ detail: "ERROR", summary: this.msg, duration: 5000, sticky: true, position: 'topRight' });
            } else {
              this.toast.info({ detail: "INFO", summary: this.msg, duration: 5000, sticky: true, position: 'topRight' });
            }

            this.clearFile();
          },
          error: (err: any) => {
            this.toast.error({ detail: "ERROR", summary: err, duration: 5000, sticky: true, position: 'topRight' });
            this.isButtonDisabled = false;
            this.showSpinner = false;
          }
        });
    }
  }

  showHistory(): void {
    if (!this.erpCustomerID) {
      this.showInfoToast('Please provide ERPCustomerID');
      return;
    }

    this.api.getHistoryCustomerProductCatalog(this.erpCustomerID).subscribe({
      next: (res: any) => {
        if (res.code === 200) {
          this.openHistoryDialog(res.customerProductCatalog_Log);
        } else {
          this.showInfoToast('No history data found');
        }
      },
      error: (err: any) => {
        this.showErrorToast('Error fetching history data');
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
