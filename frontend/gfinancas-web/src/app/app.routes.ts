import { Routes } from '@angular/router';

import { LoginComponent }
  from './features/auth/login/login.component';

import { DashboardComponent }
  from './features/dashboard/dashboard.component';

import { TransacoesComponent }
  from './features/transacoes/transacoes.component';

import { NovaTransacaoComponent }
  from './features/transacoes/nova-transacao/nova-transacao.component';

import { EditarTransacaoComponent }
  from './features/transacoes/editar-transacao/editar-transacao.component';

import { MainLayoutComponent }
  from './layouts/main-layout/main-layout.component';

import { authGuard }
  from './core/guards/auth.guard';

import { ContasComponent }
  from './features/contas/contas.component';

import { NovaContaComponent }
  from './features/contas/nova-conta/nova-conta.component';

import { EditarContaComponent }
  from './features/contas/editar-conta/editar-conta.component';

import { CategoriasComponent }
  from './features/categorias/categorias.component';

import { NovaCategoriaComponent }
  from './features/categorias/nova-categoria/nova-categoria.component';

import { EditarCategoriaComponent }
  from './features/categorias/editar-categoria/editar-categoria.component';

import { OrcamentosComponent }
  from './features/orcamentos/orcamentos.component';

import { NovoOrcamentoComponent }
  from './features/orcamentos/novo-orcamento/novo-orcamento.component';

import { EditarOrcamentoComponent }
  from './features/orcamentos/editar-orcamento/editar-orcamento.component';

import { AplicacoesListComponent }
  from './pages/aplicacoes/aplicacoes-list/aplicacoes-list.component';

import { AplicacaoFormComponent }
  from './pages/aplicacoes/aplicacao-form/aplicacao-form.component';

import { MovimentacaoFormComponent }
  from './pages/aplicacoes/movimentacao-form/movimentacao-form.component';

import { MovimentacoesListComponent }
  from './pages/aplicacoes/movimentacoes-list/movimentacoes-list.component';


export const routes: Routes = [

  {
    path: 'redefinir-senha',
    component: LoginComponent
  },

  {
    path: 'login',
    component: LoginComponent
  },

  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],

    children: [

      {
        path: 'dashboard',
        component: DashboardComponent,
        data: { visao: 'sintetico' }
      },

      {
        path: 'dashboard/financeiro',
        component: DashboardComponent,
        data: { visao: 'financeiro' }
      },
      {
        path: 'dashboard/patrimonio',
        component: DashboardComponent,
        data: { visao: 'patrimonio' }
      },
      {
        path: 'dashboard/orcamentos',
        component: DashboardComponent,
        data: { visao: 'orcamentos' }
      },

      {
        path: 'transacoes',
        component: TransacoesComponent
      },

      {
        path: 'transacoes/nova',
        component: NovaTransacaoComponent
      },

      {
        path: 'transacoes/editar/:id',
        component: EditarTransacaoComponent
      },

     {
      path: 'contas',
      component: ContasComponent
    },
    
    {
      path: 'contas/nova',
      component: NovaContaComponent
    },

    {
    path: 'contas',
    component: ContasComponent
  },
  
  {
    path: 'contas/nova',
    component: NovaContaComponent
  },

  {
    path: 'categorias',
    component: CategoriasComponent
  },

  {
  path: 'categorias',
  component: CategoriasComponent
},
{
  path: 'categorias/nova',
  component: NovaCategoriaComponent
},

  {
    path: 'orcamentos',
    component: OrcamentosComponent
  },

  {
    path: 'orcamentos/novo',
    component: NovoOrcamentoComponent
  },
  {
    path: 'orcamentos/editar/:id',
    component: EditarOrcamentoComponent
  },
  {
    path: 'contas/editar/:id',
    component: EditarContaComponent
  },


  {
  path: 'categorias',
  component: CategoriasComponent
},
{
  path: 'categorias/nova',
  component: NovaCategoriaComponent
},
{
  path: 'categorias/editar/:id',
  component: EditarCategoriaComponent
},

      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'aplicacoes',
        component: AplicacoesListComponent
      },

      {
        path: 'aplicacoes',
        component: AplicacoesListComponent
      },
      {
        path: 'aplicacoes/nova',
        component: AplicacaoFormComponent
      },
      {
        path: 'aplicacoes/:id/editar',
        component: AplicacaoFormComponent
      },
      {
        path: 'aplicacoes/:id/movimentar',
        component: MovimentacaoFormComponent
      },
      {
        path: 'aplicacoes/:id/historico',
        component: MovimentacoesListComponent
      }

    ]
  },

  {
    path: '**',
    redirectTo: 'dashboard'
  }

];