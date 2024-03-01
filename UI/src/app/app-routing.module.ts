import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { combineLatest } from 'rxjs';
import { AuthorizationGuard } from './authorization.guard';
import { AuthenticationGuard } from './guards/authentication.guard';
import { LoginComponent } from './login/login.component';
import { OrdersComponent } from './orders/orders.component';
import { ProfileComponent } from './profile/profile.component';
import { RegisterComponent } from './register/register.component';
import { UsersListComponent } from './users-list/users-list.component';
import { Process850Component } from './process850/process850.component';
import { CustomersComponent } from './customers/customers.component';
import { MapsComponent } from './maps/maps.component';
import { ConnectorsComponent } from './connectors/connectors.component';
import { PartnerGroupsComponent } from './partnergroups/partnergroups.component';
import { RoutesComponent } from './routes/routes.component';
import { CustomerProductCatalogComponent } from './customer-product-catalog/customer-product-catalog.component';
import { RouteTypesComponent } from './route-types/route-types.component';



const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: 'register',
    component: RegisterComponent,
  },
  {
    path: 'edi/process850',
    component: Process850Component,
    canActivate: [AuthenticationGuard],
  },
  {
    path: 'edi/all-orders',
    component: OrdersComponent,
    canActivate: [AuthorizationGuard],
  },
  {
    path: 'edi/customers',
    component: CustomersComponent,
    canActivate: [AuthorizationGuard],
  },
  {
    path: 'edi/maps',
    component: MapsComponent,
    canActivate: [AuthorizationGuard],
  },
  {
    path: 'edi/connectors',
    component: ConnectorsComponent,
    canActivate: [AuthorizationGuard],
  },
  {
     path: 'users/list',
     component: UsersListComponent,
     canActivate: [AuthorizationGuard],
  },
  {
     path: 'users/profile',
     component: ProfileComponent,
     canActivate: [AuthenticationGuard],
  },
  {
    path: 'edi/partnergroups',
    component: PartnerGroupsComponent,
    canActivate: [AuthenticationGuard],
  },
  {
    path: 'edi/routes',
    component: RoutesComponent,
    canActivate: [AuthenticationGuard],
  },
  {
    path: 'edi/customerProductCatalog',
    component: CustomerProductCatalogComponent,
    canActivate: [AuthenticationGuard],
  },
  {
    path: 'edi/routeTypes',
    component: RouteTypesComponent,
    canActivate: [AuthorizationGuard],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
