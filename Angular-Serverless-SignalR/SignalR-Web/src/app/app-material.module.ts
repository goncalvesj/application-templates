import { NgModule } from '@angular/core';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatSnackBarModule } from '@angular/material/snack-bar';

@NgModule({
  exports: [
    MatFormFieldModule,
    MatButtonModule,
    MatInputModule,
    MatListModule,
    MatIconModule,
    MatCardModule,
    MatSnackBarModule
  ]
})
export class AppMaterialModule {}
