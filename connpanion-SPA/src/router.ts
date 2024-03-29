import { Routes } from '@angular/router';
import { HomeComponent } from './app/home/home.component';
import { UserListComponent } from './app/users/user-list/user-list.component';
import { MessagesComponent } from './app/messages/messages.component';
import { ConpanionsComponent } from './app/conpanions/conpanions.component';
import { AuthGuard } from './app/_guards/auth.guard';
import { UserDetailComponent } from './app/users/user-detail/user-detail.component';
import { UserDetailResolver } from './app/_resolvers/user-detail.resolver';
import { UserListResolver } from './app/_resolvers/user-list.resolver';
import { UserEditComponent } from './app/users/user-edit/user-edit.component';
import { UserEditResolver } from './app/_resolvers/user-edit.resolver';
import { PreventUnsavedChanges } from './app/_guards/prevent-unsaved-changes.guard';
import { ConpanionsResolver } from './app/_resolvers/conpanions.resolver';
import { MessagesResolver } from './app/_resolvers/messages.resolver';

export const appRoutes: Routes = [
    { path: '', component: HomeComponent },
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            { path: 'users', component: UserListComponent, resolve: {users: UserListResolver}},
            { path: 'users/:id', component: UserDetailComponent, resolve: {user: UserDetailResolver} },
            { path: 'user/edit', component: UserEditComponent, resolve: {user: UserEditResolver}, canDeactivate: [PreventUnsavedChanges] },
            { path: 'messages', component: MessagesComponent, resolve: {messages: MessagesResolver} },
            { path: 'conpanions', component: ConpanionsComponent, resolve: {users: ConpanionsResolver} }
        ]
    },
    { path: '**', redirectTo: '', pathMatch: 'full' },
];
