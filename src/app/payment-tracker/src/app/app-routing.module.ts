import { NgModule } from '@angular/core';
import { PreloadAllModules, RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'payment-set/current',
    pathMatch: 'full'
  },
  {
    path: 'folder/:id',
    loadChildren: () => import('./folder/folder.module').then( m => m.FolderPageModule)
  },
  {
    path: 'template',
    loadChildren: () => import('./pages/template/template.module').then( m => m.TemplatePageModule)
  },
  {
    path: 'login',
    loadChildren: () => import('./pages/login/login.module').then( m => m.LoginPageModule)
  },
  {
    path: 'payment-set',
    loadChildren: () => import('./pages/payment-set/payment-set.module').then( m => m.PaymentSetPageModule)
  },
  {
    path: 'payment-sets',
    loadChildren: () => import('./pages/payment-sets/payment-sets.module').then( m => m.PaymentSetsPageModule)
  }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, { preloadingStrategy: PreloadAllModules })
  ],
  exports: [RouterModule]
})
export class AppRoutingModule {}
