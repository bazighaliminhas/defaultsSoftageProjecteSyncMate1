import { Component, OnInit } from '@angular/core';
import { ApiService } from '../services/api.service';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';

export interface TableElement {
  name: string;
  value: string | undefined;
}

@Component({
    selector: 'profile',
    templateUrl: './profile.component.html',
    styleUrls: ['./profile.component.scss'],
    standalone: true,
    imports: [MatCardModule, MatTableModule],
})
export class ProfileComponent implements OnInit {
  dataSource: TableElement[] = [];
  columns: string[] = ['name', 'value'];

  constructor(private api: ApiService) {}

  ngOnInit() {
    let user = this.api.getTokenUserInfo();

    this.dataSource = [
      { name: 'Name', value: user?.firstName + ' ' + user?.lastName },
      { name: 'Email', value: user?.email ?? '' },
      { name: 'Mobile', value: user?.mobile },
      { name: 'Blocked', value: this.blockedStatus() },
      { name: 'Active', value: this.activeStatus() },
    ];
  }

  blockedStatus(): string {
    let bloked = 1;//this.api.getTokenUserInfo()!.blocked;
    return bloked ? 'YES, you are BLOCKED' : 'NO, you are not BLOCKED!';
  }

  activeStatus(): string {
    let active = 1; //this.api.getTokenUserInfo()!.active;
    return active
      ? 'YES, your account is ACTIVE'
      : 'NO, your account is not ACTIVE!';
  }
}
