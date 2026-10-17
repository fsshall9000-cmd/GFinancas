import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../../core/services/auth.service';
import { LoginComponent } from './login.component';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let auth: jasmine.SpyObj<AuthService>;
  let router: jasmine.SpyObj<Router>;
  let route: { snapshot: { routeConfig: { path: string }; queryParamMap: ReturnType<typeof convertToParamMap> } };

  beforeEach(async () => {
    auth = jasmine.createSpyObj('AuthService', ['login', 'salvarToken', 'cadastrar', 'recuperarSenha', 'redefinirSenha']);
    router = jasmine.createSpyObj('Router', ['navigate']);
    router.navigate.and.resolveTo(true);
    route = { snapshot: { routeConfig: { path: 'login' }, queryParamMap: convertToParamMap({}) } };
    await TestBed.configureTestingModule({
      imports: [LoginComponent],
      providers: [
        { provide: AuthService, useValue: auth },
        { provide: Router, useValue: router },
        { provide: ActivatedRoute, useValue: route }
      ]
    }).compileComponents();
    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('exibe as opções de recuperar senha e cadastrar usuário', () => {
    const texto = fixture.nativeElement.textContent;
    expect(texto).toContain('Esqueceu a senha?');
    expect(texto).toContain('Cadastrar usuário');
  });

  it('bloqueia cadastro quando as senhas diferem', () => {
    component.cadastroForm.setValue({ nome: 'Teste', email: 'teste@example.com', senha: 'senha1234', confirmacao: 'outra1234' });
    component.cadastrar();
    expect(auth.cadastrar).not.toHaveBeenCalled();
    expect(component.erro).toContain('não coincidem');
  });

  it('cadastra e retorna ao login sem manter a senha', () => {
    auth.cadastrar.and.returnValue(of({ id: 1, nome: 'Teste', email: 'teste@example.com' }));
    component.modo = 'cadastro';
    component.cadastroForm.setValue({ nome: ' Teste ', email: 'teste@example.com', senha: 'senha1234', confirmacao: 'senha1234' });
    component.cadastrar();
    expect(auth.cadastrar).toHaveBeenCalledWith({ nome: 'Teste', email: 'teste@example.com', senha: 'senha1234' });
    expect(component.modo).toBe('login');
    expect(component.loginForm.controls.senha.value).toBe('');
    expect(component.cadastroForm.controls.senha.value).toBe('');
  });

  it('envia somente o e-mail ao solicitar recuperação', () => {
    auth.recuperarSenha.and.returnValue(of({ mensagem: 'Se o e-mail estiver cadastrado, você receberá um link.' }));
    component.recuperacaoForm.setValue({ email: 'teste@example.com' });
    component.recuperarSenha();
    expect(auth.recuperarSenha).toHaveBeenCalledWith('teste@example.com');
    expect(component.mensagem).toContain('Se o e-mail');
    expect(component.carregando).toBeFalse();
  });

  it('não envia recuperação com e-mail inválido', () => {
    component.recuperacaoForm.setValue({ email: 'invalido' });
    component.recuperarSenha();
    expect(auth.recuperarSenha).not.toHaveBeenCalled();
  });

  it('usa o token do link para atualizar a senha', () => {
    route.snapshot.routeConfig.path = 'redefinir-senha';
    route.snapshot.queryParamMap = convertToParamMap({ token: 'token-do-email' });
    component.ngOnInit();
    expect(component.modo).toBe('redefinicao');
    auth.redefinirSenha.and.returnValue(of({ mensagem: 'Senha atualizada.' }));
    component.redefinicaoForm.setValue({ senha: 'nova12345', confirmacao: 'nova12345' });
    component.redefinirSenha();
    expect(auth.redefinirSenha).toHaveBeenCalledWith('token-do-email', 'nova12345');
    expect(router.navigate).toHaveBeenCalledWith(['/login'], { replaceUrl: true });
    expect(component.mensagem).toBe('Senha atualizada.');
  });

  it('não redefine a senha sem token', () => {
    route.snapshot.routeConfig.path = 'redefinir-senha';
    component.ngOnInit();
    component.redefinicaoForm.setValue({ senha: 'nova12345', confirmacao: 'nova12345' });
    component.redefinirSenha();
    expect(auth.redefinirSenha).not.toHaveBeenCalled();
    expect(component.erro).toContain('Link inválido');
  });

  it('mostra a mensagem da API quando o link é rejeitado', () => {
    route.snapshot.routeConfig.path = 'redefinir-senha';
    route.snapshot.queryParamMap = convertToParamMap({ token: 'expirado' });
    component.ngOnInit();
    auth.redefinirSenha.and.returnValue(throwError(() => new HttpErrorResponse({ status: 400, error: { detail: 'O link expirou.' } })));
    component.redefinicaoForm.setValue({ senha: 'nova12345', confirmacao: 'nova12345' });
    component.redefinirSenha();
    expect(component.erro).toBe('O link expirou.');
    expect(component.carregando).toBeFalse();
  });
});
