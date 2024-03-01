import { Component } from '@angular/core';
import { DatePipe, NgIf } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { FormBuilder, ReactiveFormsModule, FormsModule } from '@angular/forms';
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
import { RouteTypesService } from '../services/route-types.service';
import { RouteType } from '../models/models';
import { AddRouteTypesDialogComponent } from './add-route-types-dialog/add-route-types-dialog.component';
import { EditRouteTypesDialogComponent } from './edit-route-types-dialog/edit-route-types-dialog.component';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef } from 'ag-grid-community';
import { HttpClientModule } from '@angular/common/http';



@Component({
  selector: 'route-types',
  templateUrl: './route-types.component.html',
  styleUrls: ['./route-types.component.scss'],
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
    AgGridAngular,
    HttpClientModule,
  ],
})


export class RouteTypesComponent {
      listOfRouteType: RouteType[] = [];
      RouteTypeToDisplay: RouteType[] = [];
      msg: string = '';
      code: number = 0;
      showSpinnerforSearch: boolean = false;
      showSpinner: boolean = false;
      options = ['Select Route Types', 'Id', 'Route Types Name', 'Description', 'Created Date'];
      selectedOption: string = 'Select Route Types';
      searchValue: string = '';
      startDate: string = '';
      endDate: string = '';
      showDataColumn: boolean = true;
  constructor(private api: RouteTypesService, private fb: FormBuilder, private toast: NgToastService, private dialog: MatDialog) {
  }


}
//export class RouteTypesComponent {

//    listOfRouteType: RouteType[] = [];
//    RouteTypeToDisplay: RouteType[] = [];
//    msg: string = '';
//    code: number = 0;
//    showSpinnerforSearch: boolean = false;
//    showSpinner: boolean = false;
//    options = ['Select Route Types', 'Id', 'Route Types Name', 'Description', 'Created Date'];
//    selectedOption: string = 'Select Route Types';
//    searchValue: string = '';
//    startDate: string = '';
//    endDate: string = '';
//    showDataColumn: boolean = true;



//    columns: string[] = [
//        'id',
//        'Name',
//        'Description',
//        'CreatedDate',
//        'Edit',
//    ];


//    constructor(private api: RouteTypesService, private fb: FormBuilder, private toast: NgToastService, private dialog: MatDialog) {
//    }

//    ngOnInit(): void {
//        if (this.selectedOption === 'Select Route Types') {
//            this.getRouteTypes();
//        }
//    }

//    openAddRouteTypeDialog(): void {
//        const dialogRef = this.dialog.open(AddRouteTypesDialogComponent, {
//            width: '800px'
//        });

//        dialogRef.afterClosed().subscribe(result => {
//            if (result === 'saved') {
//                this.getRouteTypes();
//            }
//        });
//    }

//    openEditDialog(connectorData: any) {
//        const dialogRef = this.dialog.open(EditRouteTypesDialogComponent, {
//            width: '800px',
//            data: connectorData
//        });

//        dialogRef.afterClosed().subscribe(result => {
//            if (result === 'updated') {
//                this.getRouteTypes();
//            }
//        });
//    }

//    get label(): string {
//        return this.selectedOption === 'Select Route Types' ? 'Select Route Types' : this.selectedOption;
//    }

//    onSelectionChange() {
//        this.searchValue = '';
//    }

//    getFormattedDate(date: any) {
//        let year = date.getFullYear();
//        let month = (1 + date.getMonth()).toString().padStart(2, '0');
//        let day = date.getDate().toString().padStart(2, '0');

//        return year + '-' + month + '-' + day;
//    }


//    getRouteTypes() {
//        this.showSpinnerforSearch = false;
//        let stringFromDate = '';
//        let stringToDate = '';

//        if (this.selectedOption === 'Select Route Types') {
//            this.searchValue = 'ALL';
//        }

//        if (this.selectedOption === 'Created Date' && this.startDate.toLocaleString().length > 10) {
//            stringFromDate = this.getFormattedDate(this.startDate);
//        }
//        if (this.selectedOption === 'Created Date' && this.endDate.toLocaleString().length > 10) {
//            stringToDate = this.getFormattedDate(this.endDate);
//        }
//        if (this.selectedOption === 'Created Date' && this.startDate.toLocaleString().length > 10 && this.endDate.toLocaleString().length > 10) {
//            this.searchValue = stringFromDate + '/' + stringToDate;
//        }

//        this.api.getRouteTypes(this.selectedOption, this.searchValue).subscribe({
//            next: (res: any) => {
//                this.listOfRouteType = res.routeType;
//                this.msg = res.message;
//                this.code = res.code;

//                this.RouteTypeToDisplay = this.listOfRouteType;

//                if (this.listOfRouteType == null || this.listOfRouteType.length === 0) {
//                    this.toast.info({ detail: "INFO", summary: 'No data found against this search filters', duration: 5000, /*sticky: true,*/ position: 'topRight' });
//                    this.showSpinnerforSearch = false;

//                    return;
//                }

//                if (this.code === 200) {
//                    this.showSpinnerforSearch = false;
//                }
//                else if (this.code === 400) {
//                    this.toast.error({ detail: "ERROR", summary: this.msg, duration: 5000, /*sticky: true,*/ position: 'topRight' });
//                    this.showSpinnerforSearch = false;
//                } else {
//                    this.toast.info({ detail: "INFO", summary: this.msg, duration: 5000, /*sticky: true,*/ position: 'topRight' });
//                    this.showSpinnerforSearch = false;
//                }

//                this.showSpinnerforSearch = false;
//            },
//            error: (err: any) => {
//                this.toast.error({ detail: "ERROR", summary: err.message, duration: 5000, /*sticky: true,*/ position: 'topRight' });
//                this.showSpinnerforSearch = false;
//            },
//        });
//    }

//}
