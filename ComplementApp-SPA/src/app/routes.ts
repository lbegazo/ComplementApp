import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { MemberListResolver } from './_resolvers/memberlist.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolvers/member-edit-resolver';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';
import { UsuarioDetalleComponent } from './Usuario/usuario-detalle/usuario-detalle.component';
import { UsuarioDetalleResolver } from './_resolvers/usuario-detalle.resolver';

export const appRoutes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  {
    path: '',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    children: [
      {
        path: 'usuario/:id',
        component: UsuarioDetalleComponent,
        resolve: { usuario: UsuarioDetalleResolver },
      },
      {
        path: 'members',
        component: MemberListComponent,
        resolve: { users: MemberListResolver },
      },
      {
        path: 'members/:id',
        component: MemberDetailComponent,
        resolve: { user: MemberDetailResolver },
      },
      {
        path: 'member/edit',
        component: MemberEditComponent,
        resolve: { user: MemberEditResolver },
        canDeactivate: [PreventUnsavedChanges]
      },
      { path: 'messages', component: MessagesComponent },
      { path: 'lists', component: ListsComponent },
    ],
  },
  { path: '**', redirectTo: 'home', pathMatch: 'full' },
];

// {path: '', redirectTo: '/recipes', pathMatch: 'full'},
// {path: 'recipes', component: RecipesComponent,
//     children:[
//               {path:'', component: RecipeStartComponent},
//               {path:'new', component: RecipeEditComponent},
//               {path:':id', component: RecipeDetailComponent},
//               {path:':id/edit', component: RecipeEditComponent}
//             ]
// },
// {path: 'shoppingList', component: ShoppingListComponent}
