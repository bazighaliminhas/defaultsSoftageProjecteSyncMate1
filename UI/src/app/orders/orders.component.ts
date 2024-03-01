import { Component, OnInit, Pipe, PipeTransform } from '@angular/core';
import { Order } from '../models/models';
import { ApiService } from '../services/api.service';
import { DatePipe, NgIf, formatDate } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { FormGroup, FormControl, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { NgToastService } from 'ng-angular-popup';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatIconModule } from '@angular/material/icon';
import { PopupComponent } from '../popup/popup.component';
import { MatDialog } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { StoresOrderComponent } from '../stores-order/stores-order.component';
import { CommonModule } from '@angular/common';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss'],
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
  ],
})
export class OrdersComponent implements OnInit {
  listOfOrders: Order[] = [];
  ordersToDisplay: Order[] = [];
  OrderForm: FormGroup;
  msg: string = '';
  code: number = 0;
  showSpinnerforSearch: boolean = false;
  showSpinner: boolean = false;
  listOfStoresOrder: Order[] = [];
  statusOptions = ['Select Status','ACKNOWLEGED','ASNGEN','ASNMARK','COMPLETE','FINISHED','INVEDI','INVOICED','NEW','PROCESSED','SYNCERROR','SYNCED','SPLITED' ];

  columns: string[] = [
    'id',
    'Status',
    'CustomerName',
    'OrderDate',
    'CreatedDate',
    'OrderNumber',
    'File',
    'SendOrderStatus',
    'GenerateASN',
    'MarkOrderforASN',
    'CreateInvoice',
    'Create810',
    'SyncOrder',
    'GetStoresOrder',
  ];

  constructor(private api: ApiService, private fb: FormBuilder, private toast: NgToastService, private dialog: MatDialog) {
    const sevenDaysAgo = new Date();
    sevenDaysAgo.setDate(sevenDaysAgo.getDate() - 7);
    this.OrderForm = this.fb.group({
      orderId: fb.control(''),
      fromDate: new FormControl(formatDate(sevenDaysAgo, "yyyy-MM-dd", "en")),
      toDate: new FormControl(formatDate(new Date(), "yyyy-MM-dd", "en")),
      orderNo: fb.control(''),
      status: fb.control('')
    });
  }

  ngOnInit(): void {
    this.getOrders();
  }

  getStatusClass(status: string): string {
    if (status === 'NEW') {
      return 'new-status';
    } else if (status === 'SYNCERROR') {
      return 'syncerror-status';
    } else if (status === 'SYNCED') {
      return 'sysced-status';
    } else if (status === 'PROCESSED') {
      return 'processed-status';
    } else if (status === 'ACKNOWLEDGED') {
      return 'acknowledged-status';
    } else if (status === 'ASNGEN') {
      return 'asngen-status';
    } else if (status === 'ASNMARK') {
      return 'asnmark-status';
    } else if (status === 'INVOICED') {
      return 'invoiced-status';
    } else if (status === 'COMPLETE') {
      return 'complete-status';
    } else if (status === 'FINISHED') {
      return 'finished-status';
    } else if (status === 'SPLITED') {
      return 'splited-status';
    } else if (status === 'INVEDI') {
      return 'invedi-status';
    } else {
      return ''; 
    }
  }


  syncOrder(element: any) {
    let orderId = element.id;
    this.showSpinner = true;

    this.api.syncOrder(orderId).subscribe({
      next: (res: any) => {
        this.msg = res.message;
        this.code = res.code;

        if (this.code === 200) {
          this.toast.success({ detail: "SUCCESS", summary: this.msg, duration: 5000, position: 'topRight' });
          this.getOrders();
          this.showSpinner = false;
        }
        else if (this.code === 400) {
          this.toast.error({ detail: "ERROR", summary: this.msg, duration: 5000, /*sticky: true,*/ position: 'topRight' });
          this.showSpinner = false;
        } else {
          this.toast.info({ detail: "INFO", summary: this.msg, duration: 5000, /*sticky: true,*/ position: 'topRight' });
          this.showSpinner = false;
        }

        this.showSpinner = false;
      },
      error: (err: any) => {
        this.toast.error({ detail: "ERROR", summary: err.message, duration: 5000, /*sticky: true,*/ position: 'topRight' });
        this.showSpinner = false;
      },
    });
  }

  //process856
  generateASN(element: any) {
    let orderId = element.id;
    this.showSpinner = true;

    this.api.generateASN(orderId).subscribe({
      next: (res: any) => {
        this.msg = res.message;
        this.code = res.code;

        if (this.code === 200) {
          this.toast.success({ detail: "SUCCESS", summary: this.msg, duration: 5000, position: 'topRight' });
          this.getOrders();
          this.showSpinner = false;
        }
        else if (this.code === 400) {
          this.toast.error({ detail: "ERROR", summary: this.msg, duration: 5000, /*sticky: true,*/ position: 'topRight' });
          this.showSpinner = false;
        } else {
          this.toast.info({ detail: "INFO", summary: this.msg, duration: 5000, /*sticky: true,*/ position: 'topRight' });
          this.showSpinner = false;
        }

        this.showSpinner = false;
      },
      error: (err: any) => {
        this.toast.error({ detail: "ERROR", summary: err.message, duration: 5000, /*sticky: true,*/ position: 'topRight' });
        this.showSpinner = false;
      },
    });
  }

  generate855EDI(element: any) {
    let orderId = element.id;
    this.showSpinner = true;

    this.api.generate855EDI(orderId).subscribe({
      next: (res: any) => {
        this.msg = res.message;
        this.code = res.code;

        if (this.code === 200) {
          this.toast.success({ detail: "SUCCESS", summary: this.msg, duration: 5000, position: 'topRight' });
          this.getOrders();
          this.showSpinner = false;
        }
        else if (this.code === 400) {
          this.toast.error({ detail: "ERROR", summary: this.msg, duration: 5000, /*sticky: true,*/ position: 'topRight' });
          this.showSpinner = false;
        } else {
          this.toast.info({ detail: "INFO", summary: this.msg, duration: 5000, /*sticky: true,*/ position: 'topRight' });
          this.showSpinner = false;
        }

        this.showSpinner = false;
      },
      error: (err: any) => {
        this.toast.error({ detail: "ERROR", summary: err.message, duration: 5000, /*sticky: true,*/ position: 'topRight' });
        this.showSpinner = false;
      },
    });
  }

  markForASN(element: any) {
    let orderId = element.id;
    this.showSpinner = true;

    this.api.markForASN(orderId).subscribe({
      next: (res: any) => {
        this.msg = res.message;
        this.code = res.code;

        if (this.code === 200) {
          this.toast.success({ detail: "SUCCESS", summary: this.msg, duration: 5000, position: 'topRight' });
          this.getOrders();
          this.showSpinner = false;
        }
        else if (this.code === 400) {
          this.toast.error({ detail: "ERROR", summary: this.msg, duration: 5000, /*sticky: true,*/ position: 'topRight' });
          this.showSpinner = false;
        } else {
          this.toast.info({ detail: "INFO", summary: this.msg, duration: 5000, /*sticky: true,*/ position: 'topRight' });
          this.showSpinner = false;
        }
      },
      error: (err: any) => {
        this.toast.error({ detail: "ERROR", summary: err.message, duration: 5000, /*sticky: true,*/ position: 'topRight' });
        this.showSpinner = false;
      },
    });
  }

  createInvoice(element: any) {
    let orderId = element.id;
    this.showSpinner = true;

    this.api.createInvoice(orderId).subscribe({
      next: (res: any) => {
        this.msg = res.message;
        this.code = res.code;

        if (this.code === 200) {
          this.toast.success({ detail: "SUCCESS", summary: this.msg, duration: 5000, position: 'topRight' });
          this.getOrders();
          this.showSpinner = false;
        }
        else if (this.code === 400) {
          this.toast.error({ detail: "ERROR", summary: this.msg, duration: 5000, /*sticky: true,*/ position: 'topRight' });
          this.showSpinner = false;
        } else {
          this.toast.info({ detail: "INFO", summary: this.msg, duration: 5000, /*sticky: true,*/ position: 'topRight' });
          this.showSpinner = false;
        }
      },
      error: (err: any) => {
        this.toast.error({ detail: "ERROR", summary: err.message, duration: 5000, /*sticky: true,*/ position: 'topRight' });
        this.showSpinner = false;
      },
    });
  }

  process810(element: any) {
    let orderId = element.id;
    this.showSpinner = true;

    this.api.process810(orderId).subscribe({
      next: (res: any) => {
        this.msg = res.message;
        this.code = res.code;

        if (this.code === 200) {
          this.toast.success({ detail: "SUCCESS", summary: this.msg, duration: 5000, position: 'topRight' });
          this.getOrders();
          this.showSpinner = false;
        }
        else if (this.code === 400) {
          this.toast.error({ detail: "ERROR", summary: this.msg, duration: 5000, /*sticky: true,*/ position: 'topRight' });
          this.showSpinner = false;
        } else {
          this.toast.info({ detail: "INFO", summary: this.msg, duration: 5000, /*sticky: true,*/ position: 'topRight' });
          this.showSpinner = false;
        }
      },
      error: (err: any) => {
        this.toast.error({ detail: "ERROR", summary: err.message, duration: 5000, /*sticky: true,*/ position: 'topRight' });
        this.showSpinner = false;
      },
    });
  }

  getOrderFiles(element: any) {
    let orderId = element.id;
    this.showSpinner = false;

    this.api.getOrderFiles(orderId).subscribe({
      next: (res: any) => {
        this.listOfOrders = res.files;
        this.msg = res.message;
        this.code = res.code;

        if (this.listOfOrders.length === 0) {
          this.toast.info({ detail: "INFO", summary: 'no data found against this order.', duration: 5000, /*sticky: true,*/ position: 'topRight' });
          this.showSpinner = false;

          return;
        }

        const dialogRef = this.dialog.open(PopupComponent, {
          width: '70%',
          data: this.listOfOrders,
        });

        dialogRef.afterClosed().subscribe(result => {
          console.log('The dialog was closed');
        });

        this.showSpinner = false;
      },
      error: (err: any) => {
        this.toast.error({ detail: "ERROR", summary: err.message, duration: 5000, /*sticky: true,*/ position: 'topRight' });
        this.showSpinner = false;
      },
    });
  }

  getStoresOrder(element: any) {
    let orderId = element.id;
    this.showSpinner = false;

    this.api.getStoresOrder(orderId).subscribe({
      next: (res: any) => {
        this.listOfStoresOrder = res.orderStores;
        this.msg = res.message;
        this.code = res.code;

        if (this.listOfStoresOrder ==  null || this.listOfStoresOrder.length === 0) {
          this.toast.info({ detail: "INFO", summary: 'No data found against this Order.', duration: 5000, /*sticky: true,*/ position: 'topRight' });
          this.showSpinner = false;

          return;
        }

        const dialogRef = this.dialog.open(StoresOrderComponent, {
          width: '50%',
          data: this.listOfStoresOrder,
        });

        dialogRef.afterClosed().subscribe(result => {
          console.log('The dialog was closed');
        });

        this.showSpinner = false;
      },
      error: (err: any) => {
        this.toast.error({ detail: "ERROR", summary: err.message, duration: 5000, /*sticky: true,*/ position: 'topRight' });
        this.showSpinner = false;
      },
    });
  }

  getFormattedDate(date: any) {
    let year = date.getFullYear();
    let month = (1 + date.getMonth()).toString().padStart(2, '0');
    let day = date.getDate().toString().padStart(2, '0');

    return year + '-' + month + '-' + day;
  }

  getOrders() {
    let orderId = (this.OrderForm.get('orderId') as FormControl).value;
    let fromDate = (this.OrderForm.get('fromDate') as FormControl).value;
    let toDate = (this.OrderForm.get('toDate') as FormControl).value;
    let orderNo = (this.OrderForm.get('orderNo') as FormControl).value;
    let status = (this.OrderForm.get('status') as FormControl).value;
    let stringFromDate = '';
    let stringToDate = '';
    this.showSpinnerforSearch = true;

    if (((orderId == '' || orderId == null || orderId == 0) && (fromDate == '' || fromDate == null) && (toDate == '' || toDate == undefined) && (orderNo == '' || orderNo == 'EMPTY') && (status == '' || status == 'Select Status'))) {
      this.toast.info({ detail: "orderId", summary: 'Please enter at least one field to proceed.', duration: 5000, /*sticky: true,*/ position: 'topRight' });
      this.showSpinnerforSearch = false;
      return;
    }

    if (orderId == '') {
      orderId = 0;
    }

    if (orderNo == '') {
      orderNo = 'EMPTY'
    }

    if (status == '') {
      status = 'Select Status'
    }

    if (fromDate !== null) {
      stringFromDate = fromDate.toLocaleString();

      if (stringFromDate.length > 10) {
        stringFromDate = this.getFormattedDate(fromDate);
      }
    } else {
      stringFromDate = '1999-01-01';
    }

    if (toDate !== null) {
      stringToDate = toDate.toLocaleString();

      if (stringToDate.length > 10) {
        stringToDate = this.getFormattedDate(toDate);
      }
    } else {
      stringToDate = '1999-01-01';
    }

    this.api.getOrders(orderId, stringFromDate, stringToDate, orderNo, status).subscribe({
      next: (res: any) => {
        this.listOfOrders = res.orders;
        this.msg = res.message;
        this.code = res.code;

        this.ordersToDisplay = this.listOfOrders;

        if (this.listOfOrders == null || this.listOfOrders.length === 0) {
          this.toast.info({ detail: "INFO", summary: 'No data found against this search filters', duration: 5000, /*sticky: true,*/ position: 'topRight' });
          this.showSpinnerforSearch = false;

          return;
        }

        if (this.code === 200) {
          //this.toast.success({ detail: "SUCCESS", summary: this.msg, duration: 5000, position: 'topRight' });
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
