import React, { FunctionComponent } from 'react';

export const Home: FunctionComponent = () => {
  return (
    <div>
      <p>Welcome to this Event Sourcing demo application, built with React</p>
      <p>
        The application is a very basic conference management system where you
        can:
      </p>
      <ul>
        <li>Create a conference with a set number of seats</li>
        <li>Reserve seats for a conference</li>
        <li>Cancel reserved seats</li>
        <li>List all conferences and see available seats</li>
      </ul>
    </div>
  );
};
