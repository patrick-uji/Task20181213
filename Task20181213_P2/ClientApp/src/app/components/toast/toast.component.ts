import { Component } from '@angular/core';
@Component({
  selector: 'toast-component',
  templateUrl: './toast.component.html',
  styleUrls: ['./toast.component.css']
})
export class ToastComponent {

  private opacity: number;
  private message: string;

  constructor() {
  }

  public showMessage(message: string) {
    this.message = message;
    this.opacity = 1;
  }

  public showHttpError(httpResponse: any) {
    let error = httpResponse.error;
    let errors = error.errors
    if (errors) {
      this.message = error.title;
      for (let currError in errors) {
        this.message += `\n"${currError}": ${errors[currError][0]}`;
      }
    } else {
      this.message = error;
    }
    this.opacity = 1;
  }

}
