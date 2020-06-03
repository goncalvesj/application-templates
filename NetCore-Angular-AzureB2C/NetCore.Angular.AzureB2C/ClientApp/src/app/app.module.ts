import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';

const configurationEndpoint = '/config';

import {
  MsalModule,
  MsalGuard,
  MsalInterceptor,
  MsalConfig
} from '@azure/msal-angular';
import {
  MSAL_CONFIG,
  MsalService
} from '@azure/msal-angular/dist/msal.service';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      {
        path: '',
        component: HomeComponent,
        canActivate: [MsalGuard],
        pathMatch: 'full'
      },
      {
        path: 'counter',
        component: CounterComponent,
        canActivate: [MsalGuard]
      },
      {
        path: 'fetch-data',
        component: FetchDataComponent,
        canActivate: [MsalGuard]
      }
    ]),
    MsalModule
  ],
  providers: [
    MsalService,
    {
      provide: MSAL_CONFIG,
      useValue: getConfig()
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: MsalInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}

export function getConfig() {
  const baseUrl = window.location.protocol + '//' + window.location.host;

  const request = new XMLHttpRequest();
  request.open('GET', baseUrl + configurationEndpoint, false); // request application settings synchronous
  request.send(null);
  const response = JSON.parse(request.responseText);

  const protectedResourceMap: [string, string[]][] = [
    [baseUrl, [response.clientId]]
  ];

  const msalConfig: MsalConfig = {
    clientID: response.clientId,
    authority: response.authority,
    validateAuthority: false,
    cacheLocation: 'localStorage',
    navigateToLoginRequestUrl: false,
    popUp: false,
    consentScopes: response.scope,
    protectedResourceMap,
    correlationId: '1234',
    // level: LogLevel.Verbose,
    piiLoggingEnabled: true
  };

  return msalConfig;
}
