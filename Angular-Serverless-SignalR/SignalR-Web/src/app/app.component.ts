import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { MessageObject } from './model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'SignalR-Web';

  status = 'disconnected.';
  userName: string;
  toUser: string;
  message: string;

  messages = new Array<MessageObject>();

  constructor(
    private http: HttpClient,
    private snackBar: MatSnackBar
  ) {}

  startChat() {
    const connection = new HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}?oid=${this.userName}`, { withCredentials: false })
      .configureLogging(LogLevel.Information)
      .build();

    this.snackBar.open('Starting connection...', '', {
      horizontalPosition: 'right',
      verticalPosition: 'top',
      duration: 5000
    });

    connection.start().then(
      () => {
        this.snackBar.open('Connected!', '', {
          horizontalPosition: 'right',
          verticalPosition: 'top',
          duration: 5000
        });
        this.status = 'connected.';
      },
      () => {
        this.snackBar.open('Error!', '', {
          horizontalPosition: 'right',
          verticalPosition: 'top',
          duration: 5000
        });
        this.status = 'error occurred.';
      }
    );

    connection.on('ReceivedMessage', (data: MessageObject) => {
      this.messages.push(data);
    });
  }

  sendMessage() {
    this.http
      .post(`${environment.apiUrl}/SendMessage`, {
        text: this.message,
        userId: this.toUser
      })
      .subscribe();
  }
}
