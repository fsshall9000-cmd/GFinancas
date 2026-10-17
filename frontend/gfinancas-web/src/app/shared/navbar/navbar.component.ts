import { Component, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';

import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    RouterLink,
    RouterLinkActive
  ],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent implements OnInit {

  nomeUsuario = '';
  erroUsuario = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.authService.obterUsuarioAtual().subscribe({
      next: usuario => this.nomeUsuario = usuario.nome,
      error: () => this.erroUsuario = true
    });
  }

  sair(): void {

    this.authService.removerToken();

    this.router.navigate(['/login']);
  }
}
