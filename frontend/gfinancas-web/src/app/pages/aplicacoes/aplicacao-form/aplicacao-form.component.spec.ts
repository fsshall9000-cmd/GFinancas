import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AplicacaoFormComponent } from './aplicacao-form.component';

describe('AplicacaoFormComponent', () => {
  let component: AplicacaoFormComponent;
  let fixture: ComponentFixture<AplicacaoFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AplicacaoFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AplicacaoFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
