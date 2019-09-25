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
        switch (runtime) {
            case 0:
                run = 'CSHARP'
                break;
            case 1:
                run = 'PYTHON'
                break;
            case 2:
                run = 'NODE'
                break;
            case 3:
                run = 'GO'
                break;
            default:
                break;
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
                            <h2>Your Lambdas</h2>
                            <table id='lambdas' class="table table-hover table-mc-light-blue">
                                
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

