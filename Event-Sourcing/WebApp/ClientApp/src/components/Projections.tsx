import React, { Component, ChangeEvent } from 'react';
import { Form, FormGroup, Label, Input, Table, Col } from 'reactstrap';

interface IProps {}
type State = {
  conferences: [];
  loading: boolean;
  id: string;
  selectValue: string;
};

export class Projections extends Component<IProps, State> {
  state: State = {
    conferences: [],
    loading: true,
    id: '',
    selectValue: '',
  };  

  loadData = async (e: ChangeEvent<HTMLInputElement>) => {
    this.setState({ selectValue: e.target.value });
    if (e.target.value === '1') {
      const requestOptions = {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' },
      };
      const response = await fetch('conference', requestOptions);
      const data = await response.json();

      this.setState(() => ({
        conferences: data,
        loading: false,
      }));

      console.dir(data);
    }
  };

  renderConferenceTable(conferences: any[]) {
    return (
      <Table striped>
        <thead>
          <tr>
            <th>Id</th>
            <th>Name</th>
            <th>Available Seats</th>
          </tr>
        </thead>
        <tbody>
          {conferences.map((conferences) => (
            <tr key={conferences.id}>
              <td>{conferences.id}</td>
              <td>{conferences.name}</td>
              <td>{conferences.seats}</td>
            </tr>
          ))}
        </tbody>
      </Table>
    );
  }

  render() {
    let contents = this.renderConferenceTable(this.state.conferences);

    return (
      <div>
        <h3>Projections</h3>
        <Form>
          <FormGroup row>
            <Label for='exampleSelect' sm={2}>
              Select Projection
            </Label>
            <Col sm={10}>
              <Input
                type='select'
                name='select'
                id='exampleSelect'
                value={this.state.selectValue}
                onChange={(e) => this.loadData(e)}
              >
                <option value='0'>Please Select</option>
                <option value='1'>List Conferences</option>
              </Input>
            </Col>
          </FormGroup>
        </Form>
        <br></br>
        <br></br>
        {contents}
      </div>
    );
  }
}
