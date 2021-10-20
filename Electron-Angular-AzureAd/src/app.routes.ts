import { ModuleWithProviders }  from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AppComponent } from './app.component';

const appRoutes: Routes = [
    { component: AppComponent, path: 'home' },
    { component: AppComponent, path: '**'},
];

export const APP_ROUTING: ModuleWithProviders = RouterModule.forRoot(appRoutes);
