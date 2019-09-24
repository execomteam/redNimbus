import React from 'react';

class LambdaTable extends React.Component {
    constructor(props) {
        super(props);
        
        this.renderTableData = this.renderTableData.bind(this);
    }
    

    renderTableData() {
    return this.props.content.map((lambda, index) => {
        const { name, runtime, trigger, guid } = lambda

        let run;

        if (runtime == 0) {
            run = ' CSHARP ';
        } else if (runtime == 1) {
            run = ' PYTHON ';
        } 

        let t;

        if (trigger == 0) {
            t = ' GET ';
        } else {
            t = ' POST ';
        }

        var url = "http://localhost:65001/api/lambda/" + guid;

        return (
            <tr>
                <td>{name}</td>
                <td>{run}</td>
                <td>{t}</td>
                <td><a href={url}>URL</a></td>
            </tr>
        )
    })
}

    render() {
        return (
            <div class="container">
                <div class="row">
                    <div class="col-xs-12">
                        <div class="table-responsive-vertical shadow-z-1">
                            <table id='lambdas' class="table table-hover table-mc-light-blue">
                                <caption class="text-center">Your Lambdas:</caption>
                                <thead>
                                    <tr>
                                        <th>Name</th>
                                        <th>Runtime</th>
                                        <th>Trigger</th>
                                        <th>Guid</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {this.renderTableData()}
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        )
    }
}
export default LambdaTable;

