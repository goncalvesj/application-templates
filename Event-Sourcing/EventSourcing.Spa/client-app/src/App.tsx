import React from 'react';
import './App.css';

import { Layout } from './components/Layout';
import { Route } from 'react-router-dom';
import { Home } from './components/Home';
import { Projections } from './components/Projections';
import { Events } from './components/Events';

const App: React.FC = () => {
  return (
    <Layout>
      <Route exact path='/' component={Home} />
      <Route path='/counter' component={Events} />
      <Route path='/fetch-data' component={Projections} />
    </Layout>
  );
};

export default App;
