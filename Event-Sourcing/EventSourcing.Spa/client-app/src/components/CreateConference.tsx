import React, { Component, FormEvent } from 'react';
import { Form, FormGroup, Label, Input, Button, Col } from 'reactstrap';

interface ICreateConferenceProps {
  loadData: Function;
}
type CreateConferenceState = {
  confSeats: string;
  confName: string;
};

export class CreateConference extends Component<
  ICreateConferenceProps,
  CreateConferenceState
> {
  state: CreateConferenceState = {
    confSeats: '',
    confName: '',
  };

  refresh = () => {
    this.props.loadData();
  };

  submit = async (form: FormEvent<HTMLFormElement>) => {
    form.preventDefault();
    const model = {
      event: 'Conference.Created',
      data: {
        name: this.state.confName,
        seats: parseInt(this.state.confSeats),
      },
    };

    const requestOptions = {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(model),
    };

    const response = await fetch(
      `${process.env.REACT_APP_API_URL}CreateConference`,
      requestOptions
    );
    const data = await response.json();

    console.dir(data);

    // this.refresh();

    alert('Conference Created!');
  };

  render() {
    return (
      <div>
        <h3>Create Conference</h3>
        <Form onSubmit={(form) => this.submit(form)}>
          <FormGroup row>
            <Label for='confName' sm={2}>
              Conference Name
            </Label>
            <Col sm={10}>
              <Input
                type='text'
                name='confName'
                id='inputconfName'
                value={this.state.confName}
                onChange={(e) => this.setState({ confName: e.target.value })}
              />
            </Col>
          </FormGroup>
          <FormGroup row>
            <Label for='confSeats' sm={2}>
              Conference Seats
            </Label>
            <Col sm={10}>
              <Input
                type='number'
                name='confSeats'
                id='inputConfSeats'
                value={this.state.confSeats}
                onChange={(e) => this.setState({ confSeats: e.target.value })}
              />
            </Col>
          </FormGroup>
          <Button>Submit</Button>
        </Form>
      </div>
    );
  }
}
