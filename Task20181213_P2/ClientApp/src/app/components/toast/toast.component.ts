import { Component } from '@angular/core';
import { trigger, state, style, transition, animate } from '@angular/animations';
@Component({
  selector: 'toast-component',
  templateUrl: './toast.component.html',
  styleUrls: ['./toast.component.css'],
  animations: [
    trigger("changeVisibility", [
      state("void", style({
        opacity: 0
      })),
      state("visible", style({
        opacity: 1
      })),
      state("hidden", style({
        opacity: 0
      })),
      transition("void => visible", animate("350ms")),
      transition("visible => hidden", animate("600ms 2s")),
      transition("hidden => visible", animate("350ms"))
    ])
  ]
})
export class ToastComponent {

  public message: string;
  private visible: boolean;

  constructor() {
  }

  public showMessage(message: string) {
    this.message = message;
    this.visible = true;
  }

  public showHttpError(httpResponse: any) {
    let error = httpResponse.error;
    let errors = error.errors
    if (errors) {
      this.message = error.title;
      for (let currError in errors) {
        let errorDescription = errors[currError][0];
        this.message += `\n"${currError}": ${errorDescription}`;
      }
    } else {
      this.message = error;
    }
    this.visible = true;
  }

  public getVisibilityState(): string {
    return this.visible ? "visible" : "hidden";
  }

  public visibilityAnimationDone(event: any) {
    if (event.toState == "visible") {
      //We've just becomed visible, let's start transitioning to the invisible state
      this.visible = false;
    }
  }

}
