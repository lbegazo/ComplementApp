import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';

export const appRoutes: Routes = [
  { path: '', component: HomeComponent },
  {
    path: '',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    children: [
      { path: 'members', component: MemberListComponent },
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
