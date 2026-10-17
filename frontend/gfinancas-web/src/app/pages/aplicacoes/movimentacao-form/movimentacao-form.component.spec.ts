import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MovimentacaoFormComponent } from './movimentacao-form.component';

describe('MovimentacaoFormComponent', () => {
  let component: MovimentacaoFormComponent;
  let fixture: ComponentFixture<MovimentacaoFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MovimentacaoFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MovimentacaoFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
