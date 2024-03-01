export interface SideNavItem {
  title: string;
  link: string;
}

export enum UserType {
  ADMIN,
  USER,
}

export interface User {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  mobile: string;
  password: string;
  status: string;
  createdDate: string;
  userType: UserType;
}

export interface Order {
  Id:	number,
  Status:	string,
  CustomerId: string,
  OrderDate: Date,
  OrderNumber: string,
  VendorNumber: string,
  OrderType: string,
  ReferenceNo: string,
  CustomerOrderNo: string,
  ExternalId: string,
  ShippingMethod: string,
  ShipToId: string,
  ShipToName: string,
  ShipToAddress1: string,
  ShipToAddress2: string,
  ShipToCity: string,
  ShipToState: string,
  ShipToZip: string,
  ShipToCountry: string,
  ShipToEmail: string,
  ShipToPhone: string,
  BillToId: string,
  BillToName: string,
  BillToAddress1: string,
  BillToAddress2: string,
  BillToCity: string,
  BillToState: string,
  BillToZip: string,
  BillToCountry: string,
  BillToEmail: string,
  BillToPhone: string,
  BuyerId: string,
  BuyerName: string,
  BuyerAddress1: string,
  BuyerAddress2: string,
  BuyerCity: string,
  BuyerState: string,
  BuyerZip: string,
  BuyerCountry: string,
  BuyerEmail: string,
  BuyerPhone: string,
  CreatedDate: Date,
  CreatedBy: number
}


export interface Customer {
  Id: number,
  Name: string,
  ERPCustomerID: string,
  ISACustomerID: string,
  ISA810ReceiverId: string,
  Marketplace: string,
  CreatedDate: Date,
  CreatedBy: number
}

export interface Map {
  Id: number,
  Name: string,
  Data: string,
  TypeId: number,
  Map: string,
  CreatedDate: Date,
  CreatedBy: number,
  MapType: string
}

export interface RouteType {
  Id: number,
  Name: string,
  Description: string,
  CreatedDate: Date,
  CreatedBy: number,
  MapType: string
}

export interface Connector {
  Id: number,
  Name: string,
  Data: string,
  TypeId: number,
  Map: string,
  CreatedDate: Date,
  CreatedBy: number,
  ConnectorType: string,
  Party: string
}

export interface PartnerGroup {
  Id: number,
  Description: string,
  SourceParty: string,
  DestinationParty: string,
  CreatedDate: Date,
  CreatedBy: number
}

export interface Routes {
  Id: number,
  SourceParty: string,
  DestinationParty: string,
  SourceConnector: string,
  DestinationConnector: string,
  PartyGroup: string,
  RouteType: string,
  CreatedDate: Date,
  CreatedBy: number
}

export interface CustomerProductCatalog {
  Id: number,
  ERPCustomerID: string,
  TCIN: string,
  PartnerSKU: string,
  ProductTitle: string,
  ItemType: string,
  Relationship: string,
  PublishStatus: string,
  DataUpdatesStatus: string,
  CreatedDate: Date,
  CreatedBy: number
}

export interface HistoryCustomerProductCatalog {
  ERPCustomerID: string,
  FileName: string,
  CreatedDate: Date,
  CreatedBy: number
}
