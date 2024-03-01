import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
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
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';
import { MatSelectModule } from '@angular/material/select';
import { ConnectorsService } from '../../services/connectors.service';

interface ConnectorType {
  id: number;
  name: string;
}

@Component({
  selector: 'edit-connector-dialog',
  templateUrl: './edit-connector-dialog.component.html',
  styleUrls: ['./edit-connector-dialog.component.scss'],
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
export class EditConnectorDialogComponent implements OnInit {
  updateConnectorForm: FormGroup | any;
  connectorTypesOptions: ConnectorType[] | undefined;
  constructor(
    public dialogRef: MatDialogRef<EditConnectorDialogComponent>,
    private formBuilder: FormBuilder,
    private ConnectorsApi: ConnectorsService,
    private toast: NgToastService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) { }

  ngOnInit() {
    this.initializeForm();
  }

  initializeForm() {
    this.updateConnectorForm = this.formBuilder.group({
      id: [this.data.id, Validators.required],
      name: [this.data.name, Validators.required],
      data: [this.data.data, Validators.required],
      typeId: [this.data.typeId, Validators.required], // Add this line
    });

    this.loadConnectorTypes();
  }

  loadConnectorTypes() {
    this.ConnectorsApi.getConnectorTypesData().subscribe({
      next: (res: any) => {
        this.connectorTypesOptions = res.connectorTypes;
      },
    });
  }

  onCancel() {
    this.dialogRef.close();
  }

  updateConnector(): void {
    const customerModel = {
      id: this.updateConnectorForm.get('id')?.value,
      name: this.updateConnectorForm.get('name')?.value,
      data: this.updateConnectorForm.get('data')?.value,
      typeId: this.updateConnectorForm.get('typeId')?.value, // Add this line
    };

    if (this.updateConnectorForm.valid) {
      this.ConnectorsApi.updateConnector(customerModel).subscribe({
        next: (res) => {
          if (res.code === 100) {
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

