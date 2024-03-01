import { Component } from '@angular/core';
import { SideNavItem } from '../models/models';
import { RouterLinkActive, RouterLink } from '@angular/router';
import { NgFor, TitleCasePipe } from '@angular/common';
import { MatListModule } from '@angular/material/list';
import { Router } from '@angular/router';


@Component({
    selector: 'side-nav',
    templateUrl: './side-nav.component.html',
    styleUrls: ['./side-nav.component.scss'],
    standalone: true,
    imports: [
        MatListModule,
        NgFor,
        RouterLinkActive,
        RouterLink,
        TitleCasePipe,
    ],
})
export class SideNavComponent {
  constructor(private router: Router) { }

  sideNavContent: SideNavItem[] = [
    //{
    //   title: 'view users',
    //   link: 'users/list',
    //},
    {
      title: 'Process 850 ',
      link: 'edi/process850',
    },
    {
      title: 'Orders',
      link: 'edi/all-orders',
    },
    {
      title: 'Customers',
      link: 'edi/customers',
    },
    {
      title: 'Maps',
      link: 'edi/maps',
    },
    {
      title: 'Connectors',
      link: 'edi/connectors',
    },
    {
      title: 'Partner Groups',
      link: 'edi/partnergroups',
    },
    {
      title: 'Routes',
      link: 'edi/routes',
    },
    {
      title: 'Customer Product Catalog',
      link: 'edi/customerProductCatalog',
    },
    {
      title: 'Hangfire Dashboard',
      link: 'hangfire/dashboard',
    },
    {
      title: 'Routes Types',
      link: 'edi/routeTypes',
    },

  ];

  goToLink(option: SideNavItem)
  {
    if (option.title.toLowerCase() === 'hangfire dashboard')
    {
      window.open('http://localhost:5000/dashboard', '_blank');
    } else
    {
      this.router.navigate([option.link]);
    }
  }
}
