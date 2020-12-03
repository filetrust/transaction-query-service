import React from "react";
import SwaggerPage from "./swagger/SwaggerPage";
import "../App.css";

import {
    HashRouter as Router,
    Route,
    Switch
} from "react-router-dom";

const App = () => {
    return (
        <Router basename="/">
            <Switch>
                <Route>
                    <SwaggerPage />
                </Route>
            </Switch>
        </Router>);
};

export default App;