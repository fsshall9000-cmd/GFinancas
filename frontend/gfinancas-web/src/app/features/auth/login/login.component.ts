import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { finalize } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';

type Modo = 'login' | 'cadastro' | 'recuperacao' | 'redefinicao';
const emailValidators = [Validators.required, Validators.email, Validators.maxLength(254)];
const senhaValidators = [Validators.required, Validators.minLength(8), Validators.maxLength(128)];

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  modo: Modo = 'login';
  carregando = false;
  erro = '';
  mensagem = '';
  private token = '';

  loginForm = new FormGroup({
    email: new FormControl('', { nonNullable: true, validators: emailValidators }),
    senha: new FormControl('', { nonNullable: true, validators: [Validators.required] })
  });
  cadastroForm = new FormGroup({
    nome: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(150)] }),
    email: new FormControl('', { nonNullable: true, validators: emailValidators }),
    senha: new FormControl('', { nonNullable: true, validators: senhaValidators }),
    confirmacao: new FormControl('', { nonNullable: true, validators: [Validators.required] })
  });
  recuperacaoForm = new FormGroup({
    email: new FormControl('', { nonNullable: true, validators: emailValidators })
  });
  redefinicaoForm = new FormGroup({
    senha: new FormControl('', { nonNullable: true, validators: senhaValidators }),
    confirmacao: new FormControl('', { nonNullable: true, validators: [Validators.required] })
  });

  constructor(private authService: AuthService, private router: Router, private route: ActivatedRoute) {}

  ngOnInit(): void {
    if (this.route.snapshot.routeConfig?.path === 'redefinir-senha') {
      this.modo = 'redefinicao';
      this.token = this.route.snapshot.queryParamMap.get('token') ?? '';
      if (!this.token) this.erro = 'Link inválido. Volte ao login e solicite um novo link.';
    }
  }

  alterarModo(modo: Modo): void {
    if (this.carregando) return;
    if (this.modo === 'redefinicao') {
      this.token = '';
      void this.router.navigate(['/login'], { replaceUrl: true });
    }
    this.modo = modo;
    this.erro = '';
    this.mensagem = '';
    this.loginForm.controls.senha.reset();
    this.cadastroForm.reset();
    this.redefinicaoForm.reset();
  }

  entrar(): void {
    if (this.carregando || this.loginForm.invalid) return;
    this.iniciar();
    const dados = this.loginForm.getRawValue();
    this.authService.login({ ...dados, email: dados.email.trim() })
      .pipe(finalize(() => this.carregando = false)).subscribe({
        next: response => {
          this.authService.salvarToken(response.token);
          void this.router.navigate(['/dashboard']);
        },
        error: erro => this.exibirErro(erro)
      });
  }

  cadastrar(): void {
    if (this.carregando || this.cadastroForm.invalid) return;
    const { nome, email, senha, confirmacao } = this.cadastroForm.getRawValue();
    if (senha !== confirmacao) {
      this.erro = 'As senhas não coincidem.';
      return;
    }
    this.iniciar();
    this.authService.cadastrar({ nome: nome.trim(), email: email.trim(), senha })
      .pipe(finalize(() => this.carregando = false)).subscribe({
        next: () => {
          this.modo = 'login';
          this.loginForm.setValue({ email: email.trim(), senha: '' });
          this.cadastroForm.reset();
          this.mensagem = 'Usuário cadastrado. Entre com seu e-mail e senha.';
        },
        error: erro => this.exibirErro(erro)
      });
  }

  recuperarSenha(): void {
    if (this.carregando || this.recuperacaoForm.invalid) return;
    this.iniciar();
    this.authService.recuperarSenha(this.recuperacaoForm.getRawValue().email.trim())
      .pipe(finalize(() => this.carregando = false)).subscribe({
        next: response => this.mensagem = response.mensagem,
        error: erro => this.exibirErro(erro)
      });
  }

  redefinirSenha(): void {
    if (this.carregando || this.redefinicaoForm.invalid || !this.token) return;
    const { senha, confirmacao } = this.redefinicaoForm.getRawValue();
    if (senha !== confirmacao) {
      this.erro = 'As senhas não coincidem.';
      return;
    }
    this.iniciar();
    this.authService.redefinirSenha(this.token, senha)
      .pipe(finalize(() => this.carregando = false)).subscribe({
        next: response => {
          this.modo = 'login';
          this.token = '';
          this.redefinicaoForm.reset();
          this.loginForm.controls.senha.reset();
          this.mensagem = response.mensagem;
          void this.router.navigate(['/login'], { replaceUrl: true });
        },
        error: erro => this.exibirErro(erro)
      });
  }

  private iniciar(): void {
    this.carregando = true;
    this.erro = '';
    this.mensagem = '';
  }

  private exibirErro(erro: HttpErrorResponse): void {
    this.erro = erro.status === 429
      ? 'Muitas tentativas. Aguarde um minuto e tente novamente.'
      : erro.status === 0 || erro.status >= 500
        ? 'Não foi possível acessar o serviço. Tente novamente mais tarde.'
        : erro.error?.detail ?? 'Não foi possível concluir. Confira os dados informados.';
  }
}
