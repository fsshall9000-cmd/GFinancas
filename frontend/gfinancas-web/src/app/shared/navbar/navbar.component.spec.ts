import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { AuthService } from '../../core/services/auth.service';
import { NavbarComponent } from './navbar.component';

describe('NavbarComponent', () => {
  let auth: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    auth = jasmine.createSpyObj('AuthService', ['obterUsuarioAtual', 'removerToken']);
    auth.obterUsuarioAtual.and.returnValue(of({ id: 1, nome: 'João da Silva', email: 'joao@example.com' }));
    await TestBed.configureTestingModule({
      imports: [NavbarComponent],
      providers: [provideRouter([]), { provide: AuthService, useValue: auth }]
    }).compileComponents();
  });

  it('exibe o nome retornado pelo perfil autenticado', () => {
    const fixture = TestBed.createComponent(NavbarComponent);
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('.usuario-nome').textContent).toContain('João da Silva');
    expect(auth.obterUsuarioAtual).toHaveBeenCalledTimes(1);
  });

  it('encerra o carregamento quando a consulta do perfil falha', () => {
    auth.obterUsuarioAtual.and.returnValue(throwError(() => new Error('HTTP 500')));
    const fixture = TestBed.createComponent(NavbarComponent);
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('.usuario-nome').textContent).toContain('Usuário indisponível');
    expect(fixture.nativeElement.textContent).not.toContain('Carregando usuário');
  });
});
