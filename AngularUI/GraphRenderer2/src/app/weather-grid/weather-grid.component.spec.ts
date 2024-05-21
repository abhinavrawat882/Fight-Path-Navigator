import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WeatherGridComponent } from './weather-grid.component';

describe('WeatherGridComponent', () => {
  let component: WeatherGridComponent;
  let fixture: ComponentFixture<WeatherGridComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ WeatherGridComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WeatherGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
