﻿import React from 'react'
import axios from 'axios'
import LambdaGroup from './LambdaGroup'
import LambdaNav from './LambdaNav'

class LambdaPage extends React.Component {

    constructor(props) {
        super(props);

        this.state = {
            lambdas: [],
            createModalShow: false
        }

        this.setCreateModalShow = this.setCreateModalShow.bind(this);
        this.addLambda = this.addLambda.bind(this);

        const options = {
            headers: { 'token': localStorage.getItem("token") }
        };

        axios.get("http://localhost:65001/api/lambda", options).then(
            (resp) => this.onSuccessHandler(resp),
            (resp) => this.onErrorHandler(resp)
        );
    }

    onErrorHandler(response) {
        alert("Error response: Uncovered case");
    }

    addLambda(lambda) {
        this.setState(prevState => ({
            lambdas: [...prevState.lambdas, lambda.value]
        }));
    }

    onSuccessHandler(resp) {
        var tempLambdas = [];

        for (var i = 0; i < resp.data.length; i++) {
            if (resp.data[i] === '*') {
                continue;
            }

            tempLambdas.push(resp.data[i]);

        }
        this.setState({
            lambdas: tempLambdas
        });
    }

    setCreateModalShow(value) {
        this.setState({
            createModalShow: value
        });
    }

    render() {
        return (
            <div className="container">
                <div className="row">
                    <div className="col-md-2">
                        <LambdaNav addLambda={this.addLambda} createModalShow={this.state.createModalShow} setCreateModalShow={this.setCreateModalShow} onClick={this.onClickeCreateLambda} />
                    </div>
                    <div className="col-md-10">
                        <br />
                        <LambdaGroup content={this.state.lambdas} />
                    </div>
                </div>
            </div>
        );
    }
}

export default LambdaPage;