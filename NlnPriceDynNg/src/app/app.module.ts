import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { JwtModule } from '@auth0/angular-jwt';
import { BaseUrl} from './shared/baseUrl';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { UiModule } from './modules/ui/ui.module';
import { UserRegisterFormComponent } from './user-management/components/user-register-form/user-register-form.component';
import { MaterialModule } from './modules/material/material.module';
import { UserLoginFormComponent } from './user-management/components/user-login-form/user-login-form.component';
import { ProductListComponent } from './product-management/components/product-list/product-list.component';
import { MessagesComponent } from './shared/components/messages/messages.component';
import { ProductFormComponent } from './product-management/components/product-form/product-form.component';
import { ProductPriceChartComponent } from './product-management/components/product-price-chart/product-price-chart.component';
import { HighchartsChartModule } from 'highcharts-angular';
import { ProductListItemComponent } from './product-management/components/product-list-item/product-list-item.component';
import { FlexLayoutModule } from '@angular/flex-layout';
import { PriceChartDialogComponent } from './product-management/components/price-chart-dialog/price-chart-dialog.component';
import { ProductDeleteDialogComponent } from './product-management/components/product-delete-dialog/product-delete-dialog.component';
import { MatProgressButtonsModule } from 'mat-progress-buttons';
import { InternalServerComponent } from './shared/components/error-handling/internal-server/internal-server.component';
import { ErrorHandlerService } from './shared/services/error-handler.service';
import { NotFoundComponent } from './shared/components/error-handling/not-found/not-found.component';
import { UserEmailConfirmComponent } from './user-management/components/user-email-confirm/user-email-confirm.component';
import { NumericDirective } from './directives/onlynumberdirective';
import { ProductNotificationDialogComponent } from './product-management/components/product-notification-dialog/product-notification-dialog.component';
import { UserEmailConfirmTokenComponent } from './user-management/components/user-email-confirm-token/user-email-confirm-token.component';
import { UnknownErrorComponent } from './shared/components/error-handling/unknown-error/unknown-error.component';
import { ForbiddenErrorComponent } from './shared/components/error-handling/forbidden-error/forbidden-error.component';

export function tokenGetter() {
  return localStorage.getItem('access_token');
}

@NgModule({
  declarations: [
    AppComponent,
    UserRegisterFormComponent,
    UserLoginFormComponent,
    ProductListComponent,
    MessagesComponent,
    ProductFormComponent,
    ProductPriceChartComponent,
    ProductListItemComponent,
    PriceChartDialogComponent,
    ProductDeleteDialogComponent,
    InternalServerComponent,
    NotFoundComponent,
    UserEmailConfirmComponent,
    NumericDirective,
    ProductNotificationDialogComponent,
    UserEmailConfirmTokenComponent,
    UnknownErrorComponent,
    ForbiddenErrorComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    UiModule,
    FormsModule,
    MaterialModule,
    HttpClientModule,
    HighchartsChartModule,
    FlexLayoutModule,
    MatProgressButtonsModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        // whitelistedDomains: ['localhost:4200', 'localhost:44372', 'localhost:62868'],
        whitelistedDomains: ['localhost:4200', 'localhost:62868', 'nlnrpricetracker.azurewebsites.net'],
        blacklistedRoutes: ['localhost:4200/register/']
      }
    })
  ],
  entryComponents: [PriceChartDialogComponent, ProductDeleteDialogComponent, ProductNotificationDialogComponent],
  providers: [BaseUrl, ErrorHandlerService],
  bootstrap: [AppComponent]
})
export class AppModule { }
