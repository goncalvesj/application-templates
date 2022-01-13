import React, { Component } from 'react';
import { CancelSeats } from './CancelSeats';
import { ReserveSeats } from './ReserveSeats';
import { CreateConference } from './CreateConference';

interface IEventsProps {}
type EventsState = {
  conferences: [];
};

export class Events extends Component<IEventsProps, EventsState> {
  state: EventsState = {
    conferences: [],
  };

  componentDidMount = async () => {
    this.loadData();
  };

  loadData = async () => {
    const requestOptions = {
      method: 'GET',
      headers: { 'Content-Type': 'application/json' },
    };
    const response = await fetch('conference', requestOptions);
    const data = await response.json();

    this.setState(() => ({
      conferences: data,
    }));
  };

  render() {
    return (
      <div>
        <CreateConference loadData={this.loadData}></CreateConference>
        <br></br>
        <ReserveSeats conferences={this.state.conferences}></ReserveSeats>
        <br></br>
        <CancelSeats conferences={this.state.conferences}></CancelSeats>
      </div>
    );
  }
}
