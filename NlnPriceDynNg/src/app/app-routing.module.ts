import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';

import { LayoutComponent } from './modules/ui/layout/layout.component';
import { UserRegisterFormComponent } from './user-management/components/user-register-form/user-register-form.component';
import { UserLoginFormComponent } from './user-management/components/user-login-form/user-login-form.component';
import { ProductListComponent} from './product-management/components/product-list/product-list.component';
import { ProductFormComponent} from './product-management/components/product-form/product-form.component';
import { AuthGuard } from './guards/auth.guard';
import { ProductPriceChartComponent } from './product-management/components/product-price-chart/product-price-chart.component';
import { InternalServerComponent } from './shared/components/error-handling/internal-server/internal-server.component';
import { UnknownErrorComponent } from './shared/components/error-handling/unknown-error/unknown-error.component';
import { NotFoundComponent } from './shared/components/error-handling/not-found/not-found.component';
import { ForbiddenErrorComponent } from './shared/components/error-handling/forbidden-error/forbidden-error.component';
import { UserEmailConfirmComponent } from './user-management/components/user-email-confirm/user-email-confirm.component';
import { UserEmailConfirmTokenComponent } from './user-management/components/user-email-confirm-token/user-email-confirm-token.component';

const routes: Routes = [
  { path: '', redirectTo: '/main', pathMatch: 'full' },
  { path: 'main', component: LayoutComponent },
  { path: 'register', component: UserRegisterFormComponent },
  { path: 'login', component: UserLoginFormComponent },
  { path: 'account/confirm-email', component: UserEmailConfirmComponent},
  { path: 'products', component: ProductListComponent, canActivate: [AuthGuard], runGuardsAndResolvers: 'always' },
  { path: 'products/new', component: ProductFormComponent, canActivate: [AuthGuard] },
  { path: 'account/resend-token', component: UserEmailConfirmTokenComponent},
  { path: '500', component: InternalServerComponent },
  { path: '403', component: ForbiddenErrorComponent },
  { path: 'unknown', component: UnknownErrorComponent },
  { path: '**', component: NotFoundComponent, pathMatch: 'full'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {
    onSameUrlNavigation: 'reload',
    enableTracing: false
  })],
  exports: [RouterModule],
})
export class AppRoutingModule {
}

export const routedComponents = [LayoutComponent, UserRegisterFormComponent, UserLoginFormComponent, ProductListComponent,
                                 ProductFormComponent, ProductPriceChartComponent, UserEmailConfirmTokenComponent];
