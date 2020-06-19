import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { HttpClientModule } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { HomeComponent } from './components/home/home.component';
import { CurrenciesService } from './services/currencies.service';
import { ToastComponent } from './components/toast/toast.component';
import { ExchangeRateService } from './services/exchangerate.service';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { ExchangerComponent } from './components/exchanger/exchanger.component';
import { FetchDataComponent } from './components/fetch-data/fetch-data.component';
@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    ToastComponent,
    NavMenuComponent,
    ExchangerComponent,
    FetchDataComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'exchanger', component: ExchangerComponent },
      { path: 'fetch-data', component: FetchDataComponent },
    ])
  ],
  providers: [
    ExchangeRateService,
    CurrenciesService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
