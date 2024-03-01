import { Component, inject, Inject, OnInit } from '@angular/core';
import { DatePipe, NgIf, formatDate } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { FormGroup, FormControl, FormBuilder, Validators, ReactiveFormsModule, FormsModule, FormArray } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { NgToastService } from 'ng-angular-popup';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';
import { MatSelectModule } from '@angular/material/select';
import { RoutesService } from '../../services/routes.service';
import { MatTabsModule } from '@angular/material/tabs';
import { MatChipEditedEvent, MatChipInputEvent } from '@angular/material/chips';
import { MatChipsModule } from '@angular/material/chips';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { LiveAnnouncer } from '@angular/cdk/a11y';
import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { CustomerProductCatalogService } from '../../services/customerProductCatalogDialog.service';

@Component({
  selector: 'edit-customer-product-catalog-dialog',
  templateUrl: './edit-customer-product-catalog-dialog.component.html',
  styleUrls: ['./edit-customer-product-catalog-dialog.component.scss'],
  standalone: true,
  providers: [DatePipe],
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
    MatTabsModule,
    MatCheckboxModule,
    MatChipsModule
  ],
})
export class EditCustomerProductCatalogDialogComponent implements OnInit {
  updateCustomerProductCatalogForm: FormGroup | any;
  
  constructor(
    public dialogRef: MatDialogRef<EditCustomerProductCatalogDialogComponent>,
    private formBuilder: FormBuilder,
    private customerProductCatalogApi: CustomerProductCatalogService,
    private toast: NgToastService,
    private datePipe: DatePipe,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) { }

  ngOnInit() {
    this.initializeForm();
  }


  initializeForm() {
    this.updateCustomerProductCatalogForm = this.formBuilder.group({
      id: [this.data.id, Validators.required],
      tcin: [this.data.tcin],
      partnerSKU: [this.data.partnerSKU],
      productTitle: [this.data.productTitle],
      itemType: [this.data.itemType], 
      itemTypeID: [this.data.itemTypeID], 
      relationship: [this.data.relationship], 
      publishStatus: [this.data.publishStatus], 
      dataUpdatesStatus: [this.data.dataUpdatesStatus], 
      price: [this.data.price], 
      offerPrice: [this.data.offerPrice], 
      mapPrice: [this.data.mapPrice], 
      offerDiscount: [this.data.offerDiscount], 
      priceLastUpdated: [this.data.priceLastUpdated], 
      distributionCenterName: [this.data.distributionCenterName], 
      distributionCenterID: [this.data.distributionCenterID], 
      inventory: [this.data.inventory], 
      inventoryLastUpdated: [this.data.inventoryLastUpdated], 
    });
  }

  onCancel() {
    this.dialogRef.close();
  }

  updateCustomerProductCatalog(): void {
    const mapModel = {
      id: this.updateCustomerProductCatalogForm.get('id')?.value,
      erpCustomerID: this.data.erpCustomerID,
      tcin: this.updateCustomerProductCatalogForm.get('tcin')?.value,
      partnerSKU: this.updateCustomerProductCatalogForm.get('partnerSKU')?.value,
      productTitle: this.updateCustomerProductCatalogForm.get('productTitle')?.value,
      itemType: this.updateCustomerProductCatalogForm.get('itemType')?.value, 
      itemTypeID: this.updateCustomerProductCatalogForm.get('itemTypeID')?.value, 
      relationship: this.updateCustomerProductCatalogForm.get('relationship')?.value, 
      publishStatus: this.updateCustomerProductCatalogForm.get('publishStatus')?.value, 
      dataUpdatesStatus: this.updateCustomerProductCatalogForm.get('dataUpdatesStatus')?.value, 
      price: this.updateCustomerProductCatalogForm.get('price')?.value, 
      offerPrice: this.updateCustomerProductCatalogForm.get('offerPrice')?.value, 
      mapPrice: this.updateCustomerProductCatalogForm.get('mapPrice')?.value, 
      offerDiscount: this.updateCustomerProductCatalogForm.get('offerDiscount')?.value, 
      priceLastUpdated: this.updateCustomerProductCatalogForm.get('priceLastUpdated')?.value ? this.datePipe.transform(this.updateCustomerProductCatalogForm.get('priceLastUpdated')?.value, 'yyyy-MM-ddTHH:mm:ss') : '1900-01-01',
      distributionCenterName: this.updateCustomerProductCatalogForm.get('distributionCenterName')?.value, 
      distributionCenterID: this.updateCustomerProductCatalogForm.get('distributionCenterID')?.value, 
      inventory: this.updateCustomerProductCatalogForm.get('inventory')?.value,
      inventoryLastUpdated: this.updateCustomerProductCatalogForm.get('inventoryLastUpdated')?.value ? this.datePipe.transform(this.updateCustomerProductCatalogForm.get('inventoryLastUpdated')?.value, 'yyyy-MM-ddTHH:mm:ss') : '1900-01-01', 
    };

    if (this.updateCustomerProductCatalogForm.valid) {
      this.customerProductCatalogApi.updateCustomerProductCatalog(mapModel).subscribe({
        next: (res) => {
          if (res.code === 100)
          {
            this.toast.success({ detail: "SUCCESS", summary: res.description, duration: 5000, position: 'topRight' });

          } else if (res.code === 400) {
            this.toast.error({ detail: "ERROR", summary: res.message, duration: 5000, /*sticky: true,*/ position: 'topRight' });
          } else if (res.code === 401) {
            this.toast.warning({ detail: "WARNING", summary: res.description, duration: 5000, /*sticky: true,*/ position: 'topRight' });
          } else {
            this.toast.info({ detail: "INFO", summary: res.message, duration: 5000, /*sticky: true,*/ position: 'topRight' });
          }

          this.dialogRef.close('updated');
        },
        error: (err) => {
          this.toast.error({ detail: "ERROR", summary: err.message, duration: 5000, /*sticky: true,*/ position: 'topRight' });
        }
      });
    }
  }


}
