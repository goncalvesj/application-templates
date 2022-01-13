import React, { Component, FormEvent } from 'react';
import { Form, FormGroup, Label, Input, Button, Col } from 'reactstrap';

interface ICancelSeatsProps {
  conferences: [];
}
type CancelSeatsState = {
  conferenceSelected: boolean;
  confId: string;
  seats: string;
};

export class CancelSeats extends Component<
  ICancelSeatsProps,
  CancelSeatsState
> {
  state: CancelSeatsState = {
    conferenceSelected: true,
    confId: '',
    seats: '',
  };

  componentDidMount = async () => {};

  cancelSeats = async (form: FormEvent<HTMLFormElement>) => {
    form.preventDefault();
    const model = {
      event: 'Conference.SeatsAdded',
      data: {
        id: this.state.confId,
        seats: parseInt(this.state.seats),
      },
    };

    const requestOptions = {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(model),
    };

    const response = await fetch('Conference', requestOptions);
    if (response.status !== 200) {
      alert(await response.text());
      return;
    }

    await response.json();

    alert('Seats Cancelled!');
  };

  renderConferenceDropdown(conferences: any[]) {
    return (
      <Input
        type='select'
        name='select'
        id='exampleSelect'
        value={this.state.confId}
        onChange={(e) => {
          this.setState({
            confId: e.target.value,
            conferenceSelected: false,
          });
        }}
      >
        <option value='0'>Please Select</option>
        {conferences.map((conferences, index) => (
          <option key={index} value={conferences.id}>
            {conferences.name}
          </option>
        ))}
      </Input>
    );
  }

  render() {
    let contents = this.renderConferenceDropdown(this.props.conferences);

    return (
      <div>
        <h3>Cancel Seats</h3>
        <Form onSubmit={(form) => this.cancelSeats(form)}>
          <FormGroup row>
            <Label for='exampleSelect' sm={2}>
              Select conference
            </Label>
            <Col sm={10}>{contents}</Col>
          </FormGroup>
          <FormGroup row>
            <Label for='reserveSeat' sm={2}>
              Number
            </Label>
            <Col sm={10}>
              <Input
                type='number'
                name='reserveSeat'
                id='inputReserveSeat'
                disabled={this.state.conferenceSelected}
                value={this.state.seats}
                onChange={(e) => this.setState({ seats: e.target.value })}
              />
            </Col>
          </FormGroup>
          <Button disabled={this.state.conferenceSelected}>Reserve</Button>
        </Form>
      </div>
    );
  }
}
