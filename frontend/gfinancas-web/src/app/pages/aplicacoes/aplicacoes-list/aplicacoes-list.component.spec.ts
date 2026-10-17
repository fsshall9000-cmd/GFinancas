import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AplicacoesListComponent } from './aplicacoes-list.component';

describe('AplicacoesListComponent', () => {
  let component: AplicacoesListComponent;
  let fixture: ComponentFixture<AplicacoesListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AplicacoesListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AplicacoesListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
