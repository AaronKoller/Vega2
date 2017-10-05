import { HttpClient } from './extenders/HttpClient';
import { BatchSmsService } from './services/batchsms.service';
import * as Raven from 'raven-js'
import { AppErrorHandler } from './components/app/app.error-handler';
import { ErrorHandler } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ToastyModule } from 'ng2-toasty';
import { UniversalModule } from 'angular2-universal';

import { VehicleService } from './services/vehicle.service';
import { AppComponent } from './components/app/app.component'
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { FetchDataComponent } from './components/fetchdata/fetchdata.component';
import { CounterComponent } from './components/counter/counter.component';
import { VehicleFormComponent } from './components/vehicle-form/vehicle-form.component';
import { BatchSmsComponent } from './components/batch-sms/batch-sms.component';

import { PapaParseModule } from 'ngx-papaparse';

// import {DataTableModule} from 'primeng/components/datatable/datatable';
// import {SharedModule} from 'primeng/primeng';
//https://www.youtube.com/watch?v=wPT3K3w6JtU
Raven
.config('https://24f00cbebc664822b092e25affcbb6ad@sentry.io/213263')
.install();

@NgModule({
    bootstrap: [ AppComponent ],
    declarations: [
        AppComponent,
        NavMenuComponent,
        CounterComponent,
        FetchDataComponent,
        HomeComponent,
        VehicleFormComponent,
        BatchSmsComponent
    ],
    imports: [
        // DataTableModule,
        // SharedModule,
        PapaParseModule,
        FormsModule,
        ToastyModule.forRoot(),
        UniversalModule, // Must be first import. This automatically imports BrowserModule, HttpModule, and JsonpModule too.
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'vehicles/new', component: VehicleFormComponent},
            { path: 'vehicles/:id', component: VehicleFormComponent},
            { path: 'home', component: HomeComponent },
            { path: 'counter', component: CounterComponent },
            { path: 'fetch-data', component: FetchDataComponent },
            { path: 'batchSms', component: BatchSmsComponent},
            { path: '**', redirectTo: 'home' }
        ])
    ],
    providers: [
        { provide: ErrorHandler, useClass: AppErrorHandler},
       VehicleService,
       BatchSmsService,
       HttpClient
    ]
})
export class AppModule {
}
