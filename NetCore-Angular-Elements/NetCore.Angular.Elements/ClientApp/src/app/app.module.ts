import { BrowserModule } from '@angular/platform-browser';
import { NgModule, Injector  } from '@angular/core';
import { createCustomElement } from '@angular/elements';
import { AppComponent } from './app.component';
import { AboutComponent } from './about/about.component';
import { ContactComponent } from './contact/contact.component';

@NgModule({
  declarations: [AppComponent, AboutComponent, ContactComponent],
  imports: [BrowserModule],
  providers: [],
  entryComponents: [AppComponent, AboutComponent, ContactComponent]
})
export class AppModule {
  constructor(private injector: Injector) {}

  ngDoBootstrap() {
    const appElement = createCustomElement(AppComponent, {
      injector: this.injector
    });
    customElements.define('app-element', appElement);

    const aboutElement = createCustomElement(AboutComponent, {
      injector: this.injector
    });
    customElements.define('about-element', aboutElement);

    const contactElement = createCustomElement(ContactComponent, {
      injector: this.injector
    });
    customElements.define('contact-element', contactElement);
  }
}
