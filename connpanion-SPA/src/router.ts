import { Routes } from '@angular/router';
import { HomeComponent } from './app/home/home.component';
import { UserListComponent } from './app/user-list/user-list.component';
import { MessagesComponent } from './app/messages/messages.component';
import { ConpanionsComponent } from './app/conpanions/conpanions.component';
import { AuthGuard } from './app/_guards/auth.guard';

export const appRoutes: Routes = [
    { path: '', component: HomeComponent },
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            { path: 'users', component: UserListComponent },
            { path: 'messages', component: MessagesComponent },
            { path: 'conpanions', component: ConpanionsComponent }
        ]
    },
    { path: '**', redirectTo: '', pathMatch: 'full' },
];
